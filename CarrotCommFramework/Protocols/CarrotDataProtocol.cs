using CarrotCommFramework.Util;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CarrotCommFramework.Protocols.CarrotDataProtocolPacket;

namespace CarrotCommFramework.Protocols
{
    public class CarrotDataProtocol : ProtocolBase
    {
        public static new string Version { get; } = "CDPV1";

        public CarrotDataProtocol()
        {
        }



        public override bool TryParse(ref ReadOnlySequence<byte> buffer, out IEnumerable<Packet>? packets)
        {
            packets = null;
            List<Packet> packetsList = new();
            SequenceReader<byte> reader = new SequenceReader<byte>(buffer);

            // 处理数据流直到不完整包或结束
            while (true)
            {
                // 读取帧头
                if ((reader.Remaining < 1) || (!reader.TryPeek(0, out byte frameStart)))
                {
                    // 不完整包结构则结束
                    break;
                    //return false;
                }
                if ((reader.Remaining < 2) || (!reader.TryPeek(1, out byte protocolId)))
                {
                    // 不完整包结构则结束
                    break;
                    //return false;
                }
                int packetLen = GetPacketLength(protocolId);
                if (packetLen == -1)
                {
                    break;
                }
                if ((reader.Remaining < packetLen) || (!reader.TryPeek(packetLen - 1, out byte frameEnd)))
                {
                    // 不完整包结构则结束
                    break;
                    //return false;
                }

                var x = reader.TryReadExact(packetLen, out var pktSeq);
                if (x)
                {
                    CarrotDataProtocolPacket pkt = new(pktSeq.ToArray());
                    switch (GetCdpType(pkt.ProtocolId!.Value))
                    {
                        case CDP_TYPE.DATA:
                            pkt = new CdpDataPacket(pkt);
                            break;
                        case CDP_TYPE.ASCII:
                            pkt = new CdpMessagePacket(pkt);
                            break;
                        case CDP_TYPE.REG:
                            pkt = new CdpRegisterPacket(pkt);
                            break;
                        default:
                            break;
                    }
                    buffer = buffer.Slice(buffer.GetPosition(packetLen));
                    packetsList.Add(pkt);

                }
            }

            packets = packetsList;
            return packetsList.Count != 0;
        }

    }
}
