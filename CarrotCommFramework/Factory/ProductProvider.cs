using CarrotCommFramework.Protocols;
using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Factory
{
    public class ProductProvider
    {
        private static readonly ProductProvider current = new();
        public static ProductProvider Current => current;

        public Container Container { get; set; } = new Container();

        public ProductProvider()
        {
        }

        public void Register<TService, TImplementation>(string serviceKey)
        {
            Container.Register(typeof(TService), typeof(TImplementation), serviceKey: serviceKey);
        }

        public TService Resolve<TService>(string serviceKey)
        {
            return Container.Resolve<TService>(serviceKey);
        }
    }
}
