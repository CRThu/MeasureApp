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
    public partial class RegisterMapVM : BaseVM
    {
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        [ObservableProperty]
        private ObservableCollection<RegFile> regFiles = new();

        [ObservableProperty]
        private RegFile selectedRegFile;



        public RegisterMapVM(AppContextManager context)
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

                    SelectedRegFile = RegFiles.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
