using CarrotProtocolLib.Impl;
using CarrotProtocolLib.Interface;
using CarrotProtocolLib.Util;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Protocol
{
    public class AsciiProtocol : IProtocol
    {
        public IDevice Device { get; set; }
        public ILogger Logger { get; set; }
        private CancellationTokenSource ParseProtocolTaskCts { get; set; }
        private Task<int> ParseProtocolTask { get; set; }

        public event IProtocol.ProtocolParseErrorHandler ProtocolParseError;

        public AsciiProtocol(IDevice device, ILogger logger)
        {
            Device = device;
            Logger = logger;
        }

        public AsciiProtocol(IDevice device, ILogger logger, IProtocol.ProtocolParseErrorHandler protocolParseErrorHandler) : this(device, logger)
        {
            ProtocolParseError += protocolParseErrorHandler;
        }

        public void Start()
        {
            Device.Open();
            ParseProtocolTaskCts = new();
            ParseProtocolTask = Task.Run(() => ParseProtocolLoop(), ParseProtocolTaskCts.Token);
        }

        public void Stop()
        {
            Device.Close();
            ParseProtocolTaskCts.Cancel();
            ParseProtocolTask.Wait();
        }

        private int ParseProtocolLoop()
        {
            try
            {
                byte[] rxBuffer = new byte[65536];
                int rxBufferCursor = 0;
                int readBytes = 0;

                while (true)
                {
                    if (Device.RxByteToRead < 1)
                    {
                        Thread.Sleep(10);
                        //cts.Token.ThrowIfCancellationRequested();
                        if (ParseProtocolTaskCts.Token.IsCancellationRequested)
                            return 0;
                    }
                    else
                    {
                        readBytes = Device.RxByteToRead;
                        readBytes = readBytes > 4 ? 4 : readBytes;
                        Device.Read(rxBuffer, rxBufferCursor, readBytes);
                        int index = Array.IndexOf(rxBuffer, (byte)'\n');
                        rxBufferCursor += readBytes;
                        if (index >= 0)
                        {
                            byte[] frame = new byte[index + 1];
                            Array.Copy(rxBuffer, 0, frame, 0, frame.Length);
                            Array.Copy(rxBuffer, frame.Length, rxBuffer, 0, rxBufferCursor - frame.Length);
                            Array.Fill(rxBuffer, (byte)0x00, rxBufferCursor - frame.Length, frame.Length);
                            rxBufferCursor -= frame.Length;
                            AddLog(frame, 0, frame.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ProtocolParseError?.Invoke(ex);
                return -1;
            }
        }

        public void Send(IProtocolRecord protocol)
        {
            Send(protocol.Bytes);
        }

        public void Send(byte[] bytes)
        {
            Send(bytes, 0, bytes.Length);
        }

        public void Send(byte[] bytes, int offset, int length)
        {
            Device.Write(bytes);
            Logger.AddTx(new AsciiProtocolRecord(bytes, offset, length));
        }

        private void AddLog(byte[] bytes, int offset, int length)
        {
            AsciiProtocolRecord protocol = new AsciiProtocolRecord(bytes, offset, length);
            Logger.AddRx(protocol);
        }
    }


    public partial class AsciiProtocolRecord : ObservableObject, IProtocolRecord
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PayloadDisplay))]
        [NotifyPropertyChangedFor(nameof(Bytes))]
        private string hexDisplay;

        public string PayloadDisplay
        {
            get
            {
                return BytesEx.BytesToAscii(Bytes);
            }
            set
            {
                Bytes = BytesEx.AsciiToBytes(value);
            }
        }

        public byte[] Bytes
        {
            get
            {
                return BytesEx.HexStringToBytes(HexDisplay);
            }
            set
            {
                HexDisplay = BytesEx.BytesToHexString(value);
            }
        }

        public AsciiProtocolRecord(string payload)
        {
            Bytes = BytesEx.AsciiToBytes(payload);
        }

        public AsciiProtocolRecord(byte[] bytes, int offset, int length)
        {
            byte[] bytesNew = new byte[length];
            Array.Copy(bytes, offset, bytesNew, 0, length);
            Bytes = bytesNew;
        }
    }
}
