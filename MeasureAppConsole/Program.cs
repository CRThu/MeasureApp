using CarrotCommFramework.Sessions;
using System.Text.Json;

namespace MeasureAppConsole
{



    internal class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var x = new OptionsBuilder()
                .Add(b => b.Interface("session").Name("INST_ROOT")
                            .Add(b => b.Interface("stream").Type("TYPE1").Name("INST1"))
                            .Add(b => b.Interface("protocol").Type("TYPE2").Name("INST2"))
                            .Add(b => b.Interface("logger").Type("LOG1").Name("LOG_INST1"))
                            .Add(b => b.Interface("logger").Type("LOG2").Name("LOG_INST2")))
                .Build();

            Console.WriteLine(x);

            var xflatten = x.Flatten();
            foreach (var i in xflatten)
            {
                var json = JsonSerializer.Serialize(i.Sources);
                Console.WriteLine(json);
            }
        }
    }

}
