using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CarrotProtocolLib.Interface;

namespace CarrotProtocolLib.Impl
{
    /*
    #define CARROT_DATA_PROTOCOL_GEN(len)                              \
        typedef struct __PROTOCOL_PACKED__                             \
        {                                                              \
            uint8_t frame_start;                                       \
            uint8_t protocol_id;                                       \
            uint16_t control_flags;                                    \
            uint8_t stream_id;                                         \
            uint16_t payload_len;                                      \
            uint8_t payload[len - CARROT_PROTOCOL_DATA_PKG_BYTES];     \
            uint16_t crc16;                                            \
            uint8_t frame_end;                                         \
        }
    carrot_data_protocol_##len;                                  \
    */

    public class DeviceProtocol
    {
        public IDevice Device { get; set; }
        public ILogger Logger { get; set; }
        private CancellationTokenSource cts { get; set; }

        public DeviceProtocol(IDevice device, ILogger logger)
        {
            Device = device;
            Logger = logger;
            cts = new();
            Task<int> receiveTask = Task.Run(() => ReceivePacket(), cts.Token);
        }

        public void Stop()
        {
            cts.Cancel();
        }

        public int ReceivePacket()
        {
            byte[] frame = new byte[65536];
            while (true)
            {
                //cts.Token.ThrowIfCancellationRequested();
                if (cts.Token.IsCancellationRequested)
                    return -1;

                // 等待读帧头
                while (Device.RxByteToRead < 2)
                {
                    if (cts.Token.IsCancellationRequested)
                        return -1;
                }
                Device.Read(frame, 0, 2);
                byte protocolId = frame[1];
                int pktLength = CarrotDataProtocol.GetPacketLength(protocolId);

                // 等待读帧尾
                while (Device.RxByteToRead < pktLength - 2)
                {
                    if (cts.Token.IsCancellationRequested)
                        return -1;
                }
                Device.Read(frame, 2, pktLength - 2);

                Logger.AddRx(new CarrotDataProtocol(frame, 0, pktLength));
            }
        }

        public void Send(byte[] bytes)
        {
            Send(bytes, 0, bytes.Length);
        }

        public void Send(byte[] bytes, int offset, int length)
        {
            Device.Write(bytes);
            Logger.AddTx(new CarrotDataProtocol(bytes, offset, length));
        }
    }
}