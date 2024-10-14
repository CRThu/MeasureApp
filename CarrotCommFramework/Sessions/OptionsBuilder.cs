using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarrotCommFramework.Sessions
{
    public interface IOptionBuilder<TOption>
    {
        public Dictionary<string, string> Sources { get; set; }
        public List<IOptionBuilder<IOptions>> Builders { get; set; }

        public TOption Build();
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

        /*
        public static IOptionBuilder<IOptions> Add<TBuilder>(this IOptionBuilder<IOptions> builder, Action<TBuilder> action) where TBuilder : IOptionBuilder<IOptions>, new()
        {
            var inst = new TBuilder();
            action(inst);
            builder.Builders.Add(inst);
            return builder;
        }
        */

        public static IOptionBuilder<IOptions> Interface(this IOptionBuilder<IOptions> builder, string @interface)
        {
            builder.Sources["interface"] = @interface;
            return builder;
        }

        public static IOptionBuilder<IOptions> Type(this IOptionBuilder<IOptions> builder, string type)
        {
            builder.Sources["type"] = type;
            return builder;
        }

        public static IOptionBuilder<IOptions> Name(this IOptionBuilder<IOptions> builder, string name)
        {
            builder.Sources["name"] = name;
            return builder;
        }
    }
}
