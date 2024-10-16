using CarrotCommFramework.Factory;
using CarrotCommFramework.Protocols;
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
                            .Add(b => b.Interface("stream").Type("COM").Name("COM250").Add("port", "COM250"))
                            .Add(b => b.Interface("protocol").Type("RAPV1").Name("RAPV1_1"))
                            .Add(b => b.Interface("logger").Type("CONSOLE").Name("LOG_INST1"))
                            .Add(b => b.Interface("logger").Type("NLOG").Name("LOG_INST2"))
                            .Add(b => b.Interface("service").Type("RECV").Name("RECV1"))
                            .Add(b => b.Interface("service").Type("PARSE").Name("PARSE1")))
                .Build();

            Console.WriteLine(x);

            var xflatten = x.Flatten();
            foreach (var i in xflatten)
            {
                var json = JsonSerializer.Serialize(i.Sources);
                Console.WriteLine(json);
            }

            var s = SessionFactory.Current.CreateSession(null, x);
            //s.Services[1].Logging += (sender, args) =>
            //{
            //    RawAsciiProtocolPacket? packet = (args.Packet) as RawAsciiProtocolPacket;
            //    if (packet is not null)
            //    {
            //        Console.WriteLine($"RECV RawAsciiProtocolPacket: {packet}");
            //    }
            //    else
            //    {
            //        Console.WriteLine("RECV MSGS");
            //    }
            //};

            s.Open();
            s.Write(new RawAsciiProtocolPacket("HELLOWORLD"));

            Console.ReadKey();
        }
    }

}
