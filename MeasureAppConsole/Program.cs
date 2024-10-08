
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace MeasureAppConsole
{
    public class MyOption
    {
        public string NAME { get; set; }
        public string DESC { get; set; }
        public string CFG1 { get; set; }
        public string CFG2 { get; set; }
        public string CFG3 { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }



    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Dictionary<string, string> dic = new();
            dic["NAME"] = "MYNAME";
            dic["DESC"] = "MYDESC";
            dic["CFG1"] = "111";
            dic["CFG2"] = "222";
            dic["CFG3"] = "333";

            var services = new ServiceCollection();
            services.AddOptions<MyOption>();
            services.Configure<MyOption>("option1",o =>
            {
                o.NAME = "MyOption1";
                o.CFG1 = "1111";
            });
            services.Configure<MyOption>("option2", o =>
            {
                o.NAME = "MyOption2";
                o.CFG1 = "2222";
            });
            var provider = services.BuildServiceProvider();
            var option1 = provider.GetService<IOptionsSnapshot<MyOption>>();
            Console.WriteLine(option1.Get("option1"));
            Console.WriteLine(option1.Get("option2"));
        }
    }
}
