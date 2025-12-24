using CarrotLink.Core.Protocols.Models;
using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DryIoc;
using DryIoc.ImTools;
using MeasureApp.Messages;
using MeasureApp.Services;
using MeasureApp.Services.RegisterMap;
using Microsoft.Win32;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        private readonly DeviceManager _deviceManager;
        private readonly RegisterLogService _regLogger;
        private readonly ConfigManager _configManager;
        public DeviceManager DeviceManager => _deviceManager;
        public RegisterLogService RegLogger => _regLogger;
        public ConfigManager ConfigManager => _configManager;

        [ObservableProperty]
        private ConnectionInfo selectedDevice;

        [ObservableProperty]
        private ObservableCollection<RegFile> regFiles = new();

        private readonly SemaphoreSlim _commLock = new SemaphoreSlim(1, 1);
        private readonly int timeout = 5000;

        public RegisterEditorVM(DeviceManager deviceManager, RegisterLogService regLogger, ConfigManager configManager)
        {
            Title = "寄存器编辑器";
            ContentId = "RegisterEditor";
            _deviceManager = deviceManager;
            _regLogger = regLogger;
            _configManager = configManager;

            timeout = ConfigManager.AppConfig.RegisterRequestTimeout;

            if (ConfigManager.AppConfig.RegisterMapFilePath != null)
            {
                if (!File.Exists(ConfigManager.AppConfig.RegisterMapFilePath))
                    ConfigManager.AppConfig.RegisterMapFilePath = null;
                else
                {
                    try
                    {
                        ImportFromFile(ConfigManager.AppConfig.RegisterMapFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"寄存器映射文件({ConfigManager.AppConfig.RegisterMapFilePath})导入失败\n{ex}");
                    }
                }
            }
        }

        private void ImportFromFile(string filePath)
        {
            string ext = Path.GetExtension(filePath);
            List<RegFile> parsedRegFiles;

            if (ext == ".xls" || ext == ".xlsx")
                parsedRegFiles = Parser.ParseFromExcel(new string[] { filePath });
            else if (ext == ".txt")
                parsedRegFiles = Parser.ParseFromTxt(new string[] { filePath });
            else
                throw new NotImplementedException($"不支持的拓展名:{ext}");

            RegFiles.Clear();
            foreach (var regFile in parsedRegFiles)
            {
                RegFiles.Add(regFile);
            }
        }

        [RelayCommand]
        public void ImportRegisterMap()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog()
                {
                    Title = "选择寄存器表或定义文件",
                    Filter = "寄存器表文件 (*.xlsx;*.xls;*.txt)|*.xlsx;*.xls;*.txt|所有文件 (*.*)|*.*",
                };

                if (ofd.ShowDialog() != true)
                    return;

                ImportFromFile(ofd.FileName);
                ConfigManager.AppConfig.RegisterMapFilePath = ofd.FileName;
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

            void handler(IRegisterPacket packet, string from, string to)
            {
                if (//sender == SelectedDevice.Name && 
                    packet.Operation == RegisterOperation.ReadResult
                    && packet.Regfile == reg.Parent.Index
                    && packet.Address == reg.Address)
                {
                    tcs?.TrySetResult(packet);
                }
            }

            RegLogger.OnRegisterUpdate += handler;

            try
            {
                // send request
                await DeviceManager[SelectedDevice.Name].SendRegister(
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
                RegLogger.OnRegisterUpdate -= handler;
            }
        }

        private async Task ExecuteReadBitFieldsOperationAsync(BitsField bitsField)
        {
            TaskCompletionSource<IRegisterPacket> tcs = new TaskCompletionSource<IRegisterPacket>();

            void handler(IRegisterPacket packet, string from, string to)
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

            RegLogger.OnRegisterUpdate += handler;

            try
            {
                // send request
                await DeviceManager[SelectedDevice.Name].SendRegister(
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
                RegLogger.OnRegisterUpdate -= handler;
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
                    await DeviceManager[SelectedDevice.Name].SendRegister(
                        RegisterOperation.Write,
                        reg.Parent.Index,
                        reg.Address,
                        reg.Value ?? 0);
                }
                else if (parameter is BitsField bitsField)
                {
                    await DeviceManager[SelectedDevice.Name].SendRegister(
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
