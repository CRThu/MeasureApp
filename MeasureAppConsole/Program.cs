using System.Text.Json;

namespace MeasureAppConsole
{
    public interface IOptions
    {
        public Dictionary<string, string> Sources { get; set; }
        public List<IOptions> NestedSources { get; set; }
    }

    public interface IOptionBuilder<TOption>
    {
        public Dictionary<string, string> Sources { get; set; }
        public List<IOptionBuilder<IOptions>> Builders { get; set; }

        public TOption Build();
    }

    public class SessionOptions : IOptions
    {
        public Dictionary<string, string> Sources { get; set; } = new();
        public List<IOptions> NestedSources { get; set; } = new();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                WriteIndented = true,
            });
        }
    }

    public class SessionOptionsBuilder : IOptionBuilder<IOptions>
    {
        public Dictionary<string, string> Sources { get; set; } = new();
        public List<IOptionBuilder<IOptions>> Builders { get; set; } = new();

        public SessionOptionsBuilder()
        {
        }

        public IOptions Build()
        {
            var opt = new SessionOptions();
            foreach (var src in Sources)
            {
                opt.Sources.Add(src.Key, src.Value);
            }
            foreach (var builder in Builders)
            {
                opt.NestedSources.Add(builder.Build());
            }
            return opt;
        }
    }

    public class StreamOptions : IOptions
    {
        public Dictionary<string, string> Sources { get; set; } = new();
        public List<IOptions> NestedSources { get; set; } = new();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                WriteIndented = true,
            });
        }
    }

    public class StreamOptionsBuilder : IOptionBuilder<IOptions>
    {
        public Dictionary<string, string> Sources { get; set; } = new();
        public List<IOptionBuilder<IOptions>> Builders { get; set; } = new();

        public StreamOptionsBuilder()
        {
        }

        public IOptions Build()
        {
            var opt = new StreamOptions();
            foreach (var src in Sources)
            {
                opt.Sources.Add(src.Key, src.Value);
            }
            foreach (var builder in Builders)
            {
                opt.NestedSources.Add(builder.Build());
            }
            return opt;
        }
    }

    public static class MyOptionBuilderExtensions
    {
        public static IOptionBuilder<IOptions> Add(this IOptionBuilder<IOptions> builder, string key, string val)
        {
            builder.Sources.Add(key, val);
            return builder;
        }

        public static IOptionBuilder<IOptions> Add<TBuilder>(this IOptionBuilder<IOptions> builder, Action<TBuilder> action) where TBuilder : IOptionBuilder<IOptions>, new()
        {
            var inst = new TBuilder();
            action(inst);
            builder.Builders.Add(inst);
            return builder;
        }

        public static IOptionBuilder<IOptions> Name(this IOptionBuilder<IOptions> builder, string name)
        {
            builder.Sources.Add("name", name);
            return builder;
        }

        public static IOptionBuilder<IOptions> Type(this IOptionBuilder<IOptions> builder, string type)
        {
            builder.Sources.Add("type", type);
            return builder;
        }
    }



    internal class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var x = new SessionOptionsBuilder().Name("INST_ROOT")
                .Add<StreamOptionsBuilder>(b => b.Type("TYPE1").Name("INST1"))
                .Add<StreamOptionsBuilder>(b => b.Type("TYPE2").Name("INST2"))
                .Add<StreamOptionsBuilder>(b => b.Type("TYPE3").Name("INST3"))
                .Build();


            Console.WriteLine(x);
        }
    }

}
