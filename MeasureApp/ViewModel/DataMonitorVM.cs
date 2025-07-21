using CarrotLink.Core.Session;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DryIoc;
using MeasureApp.Model.Common;
using MeasureApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.ViewModel
{
    public partial class DataMonitorVM : BaseVM
    {
        private readonly AppContextManager _context;
        public AppContextManager Context => _context;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedData))]
        private string selectedKey;

        [ObservableProperty]
        private string addKeyText;

        public DataLogList SelectedData => GetSelectedData();

        public DataMonitorVM(AppContextManager context)
        {
            _context = context;
        }

        private DataLogList GetSelectedData()
        {
            if (SelectedKey != null && Context.DataLogger.Contains(SelectedKey))
            {
                return Context.DataLogger[SelectedKey];
            }
            return null;
        }

        [RelayCommand]
        public void AddKey()
        {
            Context.DataLogger.GetOrAddKey(AddKeyText);
            if (SelectedKey == null)
                SelectedKey = Context.DataLogger.Keys.FirstOrDefault();
        }

        [RelayCommand]
        public void RemoveSelectedKey()
        {
            if (SelectedKey != null && Context.DataLogger.Contains(SelectedKey))
            {
                Context.DataLogger.RemoveKey(SelectedKey);
            }
            SelectedKey = Context.DataLogger.Keys.FirstOrDefault();
        }

        [RelayCommand]
        public void RemoveAllKeys()
        {
            Context.DataLogger.Clear();
            SelectedKey = null;
        }

        [RelayCommand]
        public void DataTest(object param)
        {
            try
            {
                Random random = new Random();
                string paramString = Convert.ToString(param);
                switch (paramString)
                {
                    case "D+1":
                        Task.Run(() =>
                        {
                            if (SelectedKey != null)
                            {
                                Context.DataLogger.Add(SelectedKey, random.NextDouble());
                            }
                        });
                        break;
                    case "D+1M":
                        for (int i = 0; i < 100; i++)
                        {
                            Task.Run(() =>
                            {
                                if (SelectedKey != null)
                                {
                                    Int64[] ints = new Int64[10000];
                                    for (int i = 0; i < ints.Length; i++)
                                        ints[i] = random.NextInt64();
                                    Context.DataLogger.AddRange(SelectedKey, ints);
                                }
                            });
                        }
                        break;
                    case "K+1":
                        Task.Run(() =>
                        {
                            Context.DataLogger.GetOrAddKey(random.NextInt64().ToString());
                        });
                        break;
                    case "K+1K":
                        for (int i = 0; i < 10; i++)
                        {
                            Task.Run(() =>
                            {
                                for (int i = 0; i < 100; i++)
                                {
                                    Context.DataLogger.GetOrAddKey(random.NextInt64().ToString());
                                }
                            });
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.ToString());
            }
        }
    }
}
