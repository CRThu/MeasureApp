using CarrotLink.Core.Protocols.Models;
using CarrotLink.Core.Session;
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
using System.Xml.Linq;

namespace MeasureApp.ViewModel
{
    public partial class DesignRegisterEditorVM : BaseVM
    {
        [ObservableProperty]
        private ObservableCollection<RegFile> regFiles;

        public DesignRegisterEditorVM()
        {
            RegFiles = new ObservableCollection<RegFile>();
            var rf0 = new RegFile(0, "<RF0>");
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
            var rf1 = new RegFile(1, "<RF1>");
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

        private readonly SemaphoreSlim _commLock = new SemaphoreSlim(1, 1);
        private readonly int timeout = 5000;

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
                            regfile = new RegFile((uint)RegFiles.Count, regInfo[1]);
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
        public async Task ReadRouter(object parameter)
        {
            // 获取异步等待锁
            await _commLock.WaitAsync();

            try
            {
                if (parameter is RegFile regFile)
                {
                    foreach (var reg in regFile.Registers)
                    {
                        await ExecuteReadRegisterOperationAsync(reg);
                    }
                }
                else if (parameter is Register reg)
                {
                    await ExecuteReadRegisterOperationAsync(reg);
                }
                else if (parameter is BitsField bitsField)
                {
                    await ExecuteReadBitFieldsOperationAsync(bitsField);
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
            finally
            {
                _commLock.Release();
            }
        }

        private async Task ExecuteReadRegisterOperationAsync(Register reg)
        {
            TaskCompletionSource<IRegisterPacket> tcs = new TaskCompletionSource<IRegisterPacket>();

            void handler(IRegisterPacket packet, string sender)
            {
                if (//sender == SelectedDevice.Name && 
                    packet.Operation == RegisterOperation.ReadResult
                    && packet.Regfile == reg.Parent.Index
                    && packet.Address == reg.Address)
                {
                    tcs?.TrySetResult(packet);
                }
            }

            Context.RegisterLogger.OnRegisterUpdate += handler;

            try
            {
                // send request
                await Context.Devices[SelectedDevice.Name].SendRegister(
                    RegisterOperation.ReadRequest,
                    reg.Parent.Index,
                    reg.Address);

                // wait for result
                if (await Task.WhenAny(tcs.Task, Task.Delay(timeout)) == tcs.Task)
                {
                    var resultPacket = await tcs.Task;
                    reg.Value = resultPacket.Value;
                }
                else
                {
                    throw new TimeoutException($"读取寄存器(({reg.Parent.Name}::{reg.Name:X})超时");
                }
            }
            finally
            {
                Context.RegisterLogger.OnRegisterUpdate -= handler;
            }
        }

        private async Task ExecuteReadBitFieldsOperationAsync(BitsField bitsField)
        {
            TaskCompletionSource<IRegisterPacket> tcs = new TaskCompletionSource<IRegisterPacket>();

            void handler(IRegisterPacket packet, string sender)
            {
                if (//sender == SelectedDevice.Name && 
                    packet.Operation == RegisterOperation.BitsReadResult
                    && packet.Regfile == bitsField.Parent.Parent.Index
                    && packet.Address == bitsField.Parent.Address
                    && packet.StartBit == bitsField.StartBit
                    && packet.EndBit == bitsField.EndBit)
                {
                    tcs?.TrySetResult(packet);
                }
            }

            Context.RegisterLogger.OnRegisterUpdate += handler;

            try
            {
                // send request
                await Context.Devices[SelectedDevice.Name].SendRegister(
                RegisterOperation.BitsReadRequest,
                bitsField.Parent.Parent.Index,
                bitsField.Parent.Address,
                bitsField.StartBit,
                bitsField.EndBit);

                // wait for result
                if (await Task.WhenAny(tcs.Task, Task.Delay(timeout)) == tcs.Task)
                {
                    var resultPacket = await tcs.Task;
                    bitsField.Value = resultPacket.Value;
                }
                else
                {
                    throw new TimeoutException($"读取寄存器位段({bitsField.Parent.Parent.Name}::{bitsField.Parent.Name:X}::{bitsField.Name})超时");
                }
            }
            finally
            {
                Context.RegisterLogger.OnRegisterUpdate -= handler;
            }
        }

        [RelayCommand]
        public async Task WriteRouter(object parameter)
        {
            // 获取异步等待锁
            await _commLock.WaitAsync();

            try
            {
                if (parameter is Register reg)
                {
                    await Context.Devices[SelectedDevice.Name].SendRegister(
                        RegisterOperation.Write,
                        reg.Parent.Index,
                        reg.Address,
                        reg.Value ?? 0);
                }
                else if (parameter is BitsField bitsField)
                {
                    await Context.Devices[SelectedDevice.Name].SendRegister(
                        RegisterOperation.BitsWrite,
                        bitsField.Parent.Parent.Index,
                        bitsField.Parent.Address,
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
            finally
            {
                _commLock.Release();
            }
        }
    }
}
