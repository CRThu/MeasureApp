﻿using MeasureApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MeasureApp.Model.Common;

namespace MeasureApp.Model.Devices
{
    public class GPIB3458AMeasure : NotificationObjectBase
    {
        private GPIB gpib = new();
        public bool IsOpen => gpib.IsOpen;

        public int Timeout
        {
            get => gpib.Timeout;
            set => gpib.Timeout = value;
        }

        private bool _isDataAvailable;
        private bool _isReadyForInstructions;
        public bool IsDataAvailable
        {
            get
            {
                _isDataAvailable = IsOpen && ((ReadStatusByte() & 128) != 0);
                return _isDataAvailable;
            }
        }

        public bool IsReadyForInstructions
        {
            get
            {
                _isReadyForInstructions = IsOpen && ((ReadStatusByte() & 16) != 0);
                return _isReadyForInstructions;
            }
        }

        public string Open(string deviceAddr)
        {
            gpib.Open(deviceAddr);
            Timeout = 100;
            //gpib.Write("END");
            return GetID();
        }

        public void Reset()
        {
            gpib.Write("RESET");
            gpib.Write("END");
        }

        public void Dispose()
        {
            gpib.Dispose();
        }

        public byte ReadStatusByte()
        {
            return gpib.ReadStatusByte();
        }

        public string GetID()
        {
            string idRet = string.Empty;
            // SCPI PROTOCOL
            try
            {
                idRet = gpib.Query("*IDN?\n");
                string[] idnRetArr = idRet.Trim().Split(',').ToArray();
                if (idnRetArr.Length != 4)
                    throw new NotSupportedException();
                return idnRetArr[1];
            }
            catch (Exception _) { }
            // HP3458A PROTOCOL
            try
            {
                gpib.Write("END");
                idRet = gpib.Query("ID?\n");
                return idRet.Trim();
            }
            catch (Exception _) { }
            return null;
        }

        public string GetTemp()
        {
            return gpib.Query("TEMP?");
        }

        public string GetLineFreq()
        {
            return gpib.Query("LINE?");
        }

        public string GetErrorString()
        {
            return gpib.Query("ERRSTR?");
        }

        public void SetNPLC(dynamic NPLC)
        {
            gpib.Write(CommandGenerate("NPLC", NPLC));
        }

        public string GetNPLC()
        {
            return gpib.Query("NPLC?");
        }

        public string QueryCommand(string text)
        {
            return gpib.Query(text);
        }

        public void WriteCommand(string text)
        {
            gpib.Write(text);
        }

        public string ReadString()
        {
            return gpib.ReadString();
        }

        public decimal ReadDecimal()
        {
            return ConvertNumber(gpib.ReadString());
        }

        public decimal QueryDecimal(string text)
        {
            return ConvertNumber(gpib.Query(text));
        }

        public decimal MeasureDCV(decimal rangeVoltage, decimal resolutionVoltage)
        {
            // %_resolution = (actual resolution/maximum input) × 100
            if (rangeVoltage < 0)
            {
                return QueryDecimal(CommandGenerate("DCV"));
            }
            else if (resolutionVoltage < 0)
            {
                return QueryDecimal(CommandGenerate("DCV", rangeVoltage));
            }
            else
            {
                decimal resolutionParam = resolutionVoltage / rangeVoltage * 100;
                return QueryDecimal(CommandGenerate("DCV", rangeVoltage, resolutionParam));
            }
        }

        public static string CommandGenerate(string commandName, params object[] par)
        {
            // Debug.WriteLine(commandName + " " + string.Join(",", par));
            return commandName + " " + string.Join(",", par);
        }

        public static decimal ConvertNumber(string data)
        {
            try
            {
                return Convert.ToDecimal(decimal.Parse(data, System.Globalization.NumberStyles.Float));
            }
            catch (OverflowException)
            {
                // Keysight 3458A User’s Guide, Page 146, Overload indication
                return decimal.MaxValue;
            }
            catch (FormatException)
            {
                // Sometimes error: --4.559575826E+00
                return decimal.MaxValue;
            }
        }

        public void WaitForDataAvailable()
        {
            while (!IsDataAvailable)
            {
                Thread.Sleep(50);
            }
        }
    }
}
