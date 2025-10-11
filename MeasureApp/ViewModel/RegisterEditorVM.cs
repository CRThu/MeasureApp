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
using MeasureApp.Model.RegisterMap;
using MeasureApp.Services;
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
        private ObservableCollection<RegFile> regFiles = new ObservableCollection<RegFile>()
        {
            new RegFile()
            {
                Name = "<RF0>",
                Registers = new ObservableCollection<Register>()
                {
                    new Register()
                    {
                        Name = "<REG0>" ,
                        Address = 0x00,
                        BitWidth = 8,
                        BitFields = new ObservableCollection<BitsField>()
                        {
                            new BitsField() { Name = "<FIELD0>", StartBit = 0, EndBit = 3, Desc = "<[3:0]:FIELD0>" },
                            new BitsField() { Name = "<FIELD1>", StartBit = 4, EndBit = 7, Desc = "<[7:4]:FIELD1>" }
                        }
                    },
                    new Register()
                    {
                        Name = "<REG1>" ,
                        Address = 0x01,
                        BitWidth = 8,
                        BitFields = new ObservableCollection<BitsField>()
                        {
                            new BitsField() { Name = "<FIELD0>", StartBit = 0, EndBit = 0, Desc = "<[0:0]:FIELD0>" },
                            new BitsField() { Name = "<FIELD1>", StartBit = 1, EndBit = 1, Desc = "<[1:1]:FIELD1>" },
                            new BitsField() { Name = "<FIELD2>", StartBit = 2, EndBit = 2, Desc = "<[2:2]:FIELD2>" },
                            new BitsField() { Name = "<FIELD3>", StartBit = 3, EndBit = 3, Desc = "<[3:3]:FIELD3>" },
                            new BitsField() { Name = "<FIELD4>", StartBit = 4, EndBit = 4, Desc = "<[4:4]:FIELD4>" },
                            new BitsField() { Name = "<FIELD5>", StartBit = 5, EndBit = 5, Desc = "<[5:5]:FIELD5>" },
                            new BitsField() { Name = "<FIELD6>", StartBit = 6, EndBit = 6, Desc = "<[6:6]:FIELD6>" },
                            new BitsField() { Name = "<FIELD7>", StartBit = 7, EndBit = 7, Desc = "<[7:7]:FIELD7>" }
                        }
                    },
                },
            },
            new RegFile()
            {
                Name = "<RF1>",
                Registers = new ObservableCollection<Register>()
                {
                    new Register()
                    {
                        Name = "<REG0>" ,
                        Address = 0x00,
                        BitWidth = 8,
                        BitFields = new ObservableCollection<BitsField>()
                        {
                            new BitsField() { Name = "<FIELD0>", StartBit = 0, EndBit = 0, Desc = "<[0:0]:FIELD0>" },
                            new BitsField() { Name = "<FIELD1>", StartBit = 1, EndBit = 1, Desc = "<[1:1]:FIELD1>" },
                            new BitsField() { Name = "<FIELD2>", StartBit = 2, EndBit = 2, Desc = "<[2:2]:FIELD2>" },
                            new BitsField() { Name = "<FIELD3>", StartBit = 3, EndBit = 3, Desc = "<[3:3]:FIELD3>" },
                            new BitsField() { Name = "<FIELD4>", StartBit = 4, EndBit = 4, Desc = "<[4:4]:FIELD4>" },
                            new BitsField() { Name = "<FIELD5>", StartBit = 5, EndBit = 5, Desc = "<[5:5]:FIELD5>" },
                            new BitsField() { Name = "<FIELD6>", StartBit = 6, EndBit = 6, Desc = "<[6:6]:FIELD6>" },
                            new BitsField() { Name = "<FIELD7>", StartBit = 7, EndBit = 7, Desc = "<[7:7]:FIELD7>" }
                        }
                    },
                },
            }
        };
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
                    RegFile? regfile = null;
                    foreach (var regInfo in regInfos)
                    {
                        if (regInfo[0] == "+RF")
                        {
                            regfile = new RegFile()
                            {
                                Name = regInfo[1],
                            };
                            RegFiles.Add(regfile);
                        }
                        else if (regInfo[0] == "+REG")
                        {
                            if (regfile == null)
                                throw new Exception("未初始化REGFILE");
                            regfile.Registers.Add(new Register()
                            {
                                Address = Convert.ToUInt32(regInfo[1], 16),
                                BitWidth = Convert.ToUInt32(regInfo[2], 16),
                                Name = regInfo[3],
                                // Value = 0,
                            });
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
                            reg.BitFields.Add(new BitsField()
                            {
                                StartBit = startBit,
                                EndBit = endBit,
                                Name = name,
                                Desc = desc
                                // Value = 0,
                            });
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
                    MessageBox.Show($"READ REGISTER: {reg.Name}");
                }
                else if (parameter is BitsField bitsField)
                {
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
                        0/*TODO*/,
                        reg.Address,
                        reg.Value ?? 0);
                }
                else if (parameter is BitsField bitsField)
                {
                    Context.Devices[SelectedDevice.Name].SendRegister(
                        RegisterOperation.BitsWrite,
                        0/*TODO*/,
                        0/*TODO*/,
                        bitsField.StartBit,
                        bitsField.EndBit,
                        bitsField.Value ?? 0);
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
