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
            //while (true)
            //{
            // 读取帧头
            if (!reader.TryPeek(0, out byte frameStart))
            {
                // 不完整包结构则结束
                //break;
                return false;
            }
            if (!reader.TryPeek(1, out byte protocolId))
            {
                // 不完整包结构则结束
                //break;
                return false;
            }
            int packetLen = GetPacketLength(protocolId);
            if (!reader.TryPeek(packetLen - 1, out byte frameEnd))
            {
                // 不完整包结构则结束
                //break;
                return false;
            }


            CarrotDataProtocolPacket pkt = new(buffer.Slice(buffer.Start, packetLen).ToArray());
            buffer = buffer.Slice(buffer.GetPosition(packetLen));
            packetsList.Add(pkt);
            //}

            packets = packetsList;
            return packetsList.Count != 0;
        }

        public override Packet Encode(byte[] payload, byte? protocolId, byte? streamId)
        {
            return new CarrotDataProtocolPacket(payload, protocolId, streamId);
        }

    }
}
