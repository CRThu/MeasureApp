﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolCommDemo.ViewModel
{
    public class ViewModelBase : ObservableRecipient
    {
        public ViewModelBase()
        {
            // Enable Messenger Receive
            IsActive = true;
        }
    }
}
