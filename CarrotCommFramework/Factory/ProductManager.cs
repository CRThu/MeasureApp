using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Factory
{
    public class ProductManager
    {
        private static readonly ProductManager current = new();
        public static ProductManager Current => current;

    }
}
