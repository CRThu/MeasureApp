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

    public class MyOption : IOptions
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


    public class MyOptionBuilder : IOptionBuilder<IOptions>
    {
        public Dictionary<string, string> Sources { get; set; } = new();
        public List<IOptionBuilder<IOptions>> Builders { get; set; } = new();

        public MyOptionBuilder Add(string key, string val)
        {
            Sources.Add(key, val);
            return this;
        }

        public MyOptionBuilder Add<TBuilder>(Action<TBuilder> action) where TBuilder : IOptionBuilder<IOptions>, new()
        {
            var builder = new TBuilder();
            action(builder);
            Builders.Add(builder);
            return this;
        }

        public IOptions Build()
        {
            var opt = new MyOption();
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

    public class StreamOption : IOptions
    {
        public Dictionary<string, string> Sources { get; set; } = new();
        public List<IOptions> NestedSources { get; set; } = new();
    }

    public class StreamOptionBuilder : IOptionBuilder<IOptions>
    {
        public Dictionary<string, string> Sources { get; set; } = new();
        public List<IOptionBuilder<IOptions>> Builders { get; set; } = new();

        public StreamOptionBuilder Add(string key, string val)
        {
            Sources.Add(key, val);
            return this;
        }

        public IOptions Build()
        {
            var opt = new StreamOption();
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
    }



    internal class Program
    {

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var x = new MyOptionBuilder()
                .Add("ROOT", "INST_ROOT")
                .Add<StreamOptionBuilder>(b => b.Add("TYPE1", "INST1"))
                .Add<StreamOptionBuilder>(b => b.Add("TYPE1", "INST2"))
                .Add<StreamOptionBuilder>(b => b.Add("TYPE1", "INST3"))
                .Build();


            Console.WriteLine(x);
        }
    }

}
