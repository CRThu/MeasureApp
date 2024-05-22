using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOStreamDemo.Services
{
    public interface IService
    {
        public Task Task { get; set; }
        public void Run();
        public void Stop();
    }
}
