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
    public class Options : IOptions
    {
        public Dictionary<string, string> Sources { get; set; }
        public List<IOptions> NestedSources { get; set; }

        public Options()
        {
            Sources = new();
            NestedSources = new();
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                WriteIndented = true,
            });
        }
    }

    public class OptionsBuilder : IOptionBuilder<IOptions>
    {
        public Dictionary<string, string> Sources { get; set; } = new();
        public List<IOptionBuilder<IOptions>> Builders { get; set; } = new();

        public OptionsBuilder()
        {
        }

        public IOptions Build()
        {
            var opt = new Options();
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

        public static IOptionBuilder<IOptions> Add(this IOptionBuilder<IOptions> builder, Action<IOptionBuilder<IOptions>> action)
        {
            var inst = new OptionsBuilder();
            action(inst);
            builder.Builders.Add(inst);
            return builder;
        }

        public static IOptionBuilder<IOptions> Add<TBuilder>(this IOptionBuilder<IOptions> builder, Action<TBuilder> action) where TBuilder : IOptionBuilder<IOptions>, new()
        {
            var inst = new TBuilder();
            action(inst);
            builder.Builders.Add(inst);
            return builder;
        }

        public static IOptionBuilder<IOptions> Interface(this IOptionBuilder<IOptions> builder, string @interface)
        {
            builder.Sources.Add("interface", @interface);
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

            var x = new OptionsBuilder()
                .Add(b => b.Name("INST_ROOT")
                            .Add(b => b.Interface("stream").Type("TYPE1").Name("INST1"))
                            .Add(b => b.Interface("protocol").Type("TYPE2").Name("INST2"))
                            .Add(b => b.Interface("logger").Type("LOG1").Name("LOG_INST1"))
                            .Add(b => b.Interface("logger").Type("LOG2").Name("LOG_INST2")))
                .Build();

            Console.WriteLine(x);
        }
    }

}
