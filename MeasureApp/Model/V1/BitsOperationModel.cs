using MeasureApp.Model.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MeasureApp.Model
{
    public class BitsOperationModel : NotificationObjectBase
    {
        private ObservableCollection<ObservableTValue<bool>> bits = new();
        public ObservableCollection<ObservableTValue<bool>> Bits
        {
            get => bits;
            set
            {
                bits = value;
                BitsOperationModel_PropertyChanged(null, null);
            }
        }

        public uint Num
        {
            get => GetUInt32();
            set
            {
                SetUInt32(value);
                BitsOperationModel_PropertyChanged(null, null);
            }
        }

        public BitsOperationModel()
        {
            Bits = new();
            for (int i = 0; i < 32; i++)
            {
                Bits.Add(new ObservableTValue<bool>());
                Bits[i].OnValueChanged += BitsOperationModel_PropertyChanged;
            }
        }

        private void BitsOperationModel_PropertyChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(() => Bits);
            RaisePropertyChanged(() => Num);
        }

        public uint GetUInt32()
        {
            try
            {
                uint sum = 0;
                for (int i = 0; i < 32; i++)
                {
                    sum += Bits[i].Value ? (1U << i) : 0U;
                }
                return sum;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return 0xcccccccc;
            }
        }

        public void SetUInt32(uint val)
        {
            try
            {
                for (int i = 0; i < 32; i++)
                {
                    Bits[i].Value = val % 2 == 1;
                    val >>= 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
