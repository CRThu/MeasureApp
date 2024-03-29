﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Device
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceInfo
    {
        public string Interface { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Name} | {Description}";
        }

        public DeviceInfo(string @interface, string name, string description)
        {
            Interface = @interface;
            Name = name;
            Description = description;
        }
    }
}
