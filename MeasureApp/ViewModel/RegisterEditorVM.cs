using CarrotLink.Core.Protocols.Models;
using CarrotLink.Core.Session;
using CarrotLink.Core.Utility;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using DryIoc.ImTools;
using MeasureApp.Messages;
using MeasureApp.Model.Common;
using MeasureApp.Services;
using MeasureApp.Services.RegisterMap;
using Microsoft.Win32;
using NationalInstruments.NI4882;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class DesignRegisterEditorVM : BaseVM
    {
        [ObservableProperty]
        private ObservableCollection<RegFile> regFiles;

        public DesignRegisterEditorVM()
        {
            RegFiles = new ObservableCollection<RegFile>();
            var rf0 = new RegFile() { Index = 0, Name = "<RF0>" };
            rf0.AddReg("<REG0>", 0x00, 8)
                .AddBits("<FIELD0>", 0, 3, "<[3:0]:FIELD0>")
                .AddBits("<FIELD1>", 4, 7, "<[7:4]:FIELD1>")
                .AddReg("<REG1>", 0x01, 8)
                .AddBits("<FIELD0>", 0, 0, "<[0:0]:FIELD0>")
                .AddBits("<FIELD1>", 1, 1, "<[1:1]:FIELD1>")
                .AddBits("<FIELD2>", 2, 2, "<[2:2]:FIELD2>")
                .AddBits("<FIELD3>", 3, 3, "<[3:3]:FIELD3>")
                .AddBits("<FIELD4>", 4, 4, "<[4:4]:FIELD4>")
                .AddBits("<FIELD5>", 5, 5, "<[5:5]:FIELD5>")
                .AddBits("<FIELD6>", 6, 6, "<[6:6]:FIELD6>")
                .AddBits("<FIELD7>", 7, 7, "<[7:7]:FIELD7>");
            RegFiles.Add(rf0);
            var rf1 = new RegFile() { Index = 1, Name = "<RF1>" };
            rf1.AddReg("<REG0>", 0x00, 8)
                .AddBits("<FIELD0>", 0, 0, "<[0:0]:FIELD0>")
                .AddBits("<FIELD1>", 1, 1, "<[1:1]:FIELD1>")
                .AddBits("<FIELD2>", 2, 2, "<[2:2]:FIELD2>")
                .AddBits("<FIELD3>", 3, 3, "<[3:3]:FIELD3>")
                .AddBits("<FIELD4>", 4, 4, "<[4:4]:FIELD4>")
                .AddBits("<FIELD5>", 5, 5, "<[5:5]:FIELD5>")
                .AddBits("<FIELD6>", 6, 6, "<[6:6]:FIELD6>")
                .AddBits("<FIELD7>", 7, 7, "<[7:7]:FIELD7>");
            RegFiles.Add(rf1);
        }
    }

    public partial class RegisterEditorVM : BaseVM
    {
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        [ObservableProperty]
        private ConnectionInfo selectedDevice;

        [ObservableProperty]
        private ObservableCollection<RegFile> regFiles = new();


        public RegisterEditorVM(AppContextManager context)
        {
            _context = context;
        }

        [RelayCommand]
        public void ImportRegisterMap()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                };
                if (ofd.ShowDialog() == true)
                {
                    string[][] regInfos = File.ReadAllLines(ofd.FileName).Select(l => l.Split(',')).ToArray();

                    RegFiles.Clear();
                    RegFile regfile = null;
                    foreach (var regInfo in regInfos)
                    {
                        if (regInfo[0] == "+RF")
                        {
                            regfile = new RegFile()
                            {
                                Index = (uint)RegFiles.Count,
                                Name = regInfo[1],
                            };
                            RegFiles.Add(regfile);
                        }
                        else if (regInfo[0] == "+REG")
                        {
                            if (regfile == null)
                                throw new Exception("未初始化REGFILE");
                            uint addr = Convert.ToUInt32(regInfo[1], 16);
                            uint bitWidth = Convert.ToUInt32(regInfo[2], 16);
                            regfile.AddReg(regInfo[3], addr, bitWidth);
                        }
                        else
                        {
                            uint addr = Convert.ToUInt32(regInfo[0], 16);
                            uint endBit = Convert.ToUInt32(regInfo[1], 16);
                            uint startBit = Convert.ToUInt32(regInfo[2], 16);
                            string name = regInfo[3];
                            string desc = regInfo.Length >= 5 ? regInfo[4] : "<DESC>";
                            var reg = regfile?.Registers.Where(r => r.Address == addr).FirstOrDefault();
                            if (reg == null)
                                throw new Exception("未初始化REG");
                            reg.AddBits(name, startBit, endBit, desc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }


        [RelayCommand]
        public void ReadRouter(object parameter)
        {
            try
            {
                if (parameter is RegFile regFile)
                {
                    MessageBox.Show($"READ REGFILE: {regFile.Name}");
                }
                else if (parameter is Register reg)
                {
                    Context.Devices[SelectedDevice.Name].SendRegister(
                        RegisterOperation.ReadRequest,
                        reg.Parent.Index,
                        reg.Address,
                        reg.Value ?? 0).GetAwaiter().GetResult();

                    // TODO result proc
                    MessageBox.Show($"READ REGISTER: {reg.Name}");
                }
                else if (parameter is BitsField bitsField)
                {
                    Context.Devices[SelectedDevice.Name].SendRegister(
                        RegisterOperation.BitsReadRequest,
                        bitsField.Parent.Parent.Index,
                        bitsField.Parent.Address,
                        bitsField.StartBit,
                        bitsField.EndBit,
                        bitsField.Value ?? 0).GetAwaiter().GetResult();

                    // TODO result proc
                    MessageBox.Show($"READ BITSFIELD: {bitsField.Name}");
                }
                else
                {
                    throw new ArgumentException($"无法解析参数: {parameter}");
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }

        [RelayCommand]
        public void WriteRouter(object parameter)
        {
            try
            {
                if (parameter is Register reg)
                {
                    Context.Devices[SelectedDevice.Name].SendRegister(
                        RegisterOperation.Write,
                        reg.Parent.Index,
                        reg.Address,
                        reg.Value ?? 0).GetAwaiter().GetResult();
                }
                else if (parameter is BitsField bitsField)
                {
                    Context.Devices[SelectedDevice.Name].SendRegister(
                        RegisterOperation.BitsWrite,
                        bitsField.Parent.Parent.Index,
                        bitsField.Parent.Address,
                        bitsField.StartBit,
                        bitsField.EndBit,
                        bitsField.Value ?? 0).GetAwaiter().GetResult();
                }
                else
                {
                    throw new ArgumentException($"无法解析参数: {parameter}");
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
