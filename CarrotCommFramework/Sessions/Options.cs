using DryIoc.ImTools;
using NationalInstruments.Restricted;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarrotCommFramework.Sessions
{
    public interface IOptions
    {
        public Dictionary<string, string> Sources { get; set; }
        public List<IOptions> NestedSources { get; set; }

        public IEnumerable<IOptions> Flatten();
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

        public IEnumerable<IOptions> Flatten()
        {
            List<IOptions> flat = new List<IOptions>();
            flat.Add(this);
            foreach (IOptions src in NestedSources)
            {
                flat.AddRange(src.Flatten());
            }
            return flat;
        }
    }

}
