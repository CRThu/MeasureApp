using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Service
{
    /// <summary>
    /// 服务接口
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start();

        /// <summary>
        /// 结束服务
        /// </summary>
        public void Stop();
    }
}
