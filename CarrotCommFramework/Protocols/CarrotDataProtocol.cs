using CarrotCommFramework.Util;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotCommFramework.Protocols
{
    public class CarrotDataProtocol : ProtocolBase
    {
        public static new string Version { get; } = "CDPV1";

        public CarrotDataProtocol()
        {
        }

        public const byte FrameStartByte = 0x3C;
        public const byte FrameEndByte = 0x3E;

        public const byte ProtocolIdAsciiTransfer64 = 0x31;
        public const byte ProtocolIdAsciiTransfer256 = 0x32;
        public const byte ProtocolIdAsciiTransfer2048 = 0x33;
        public const byte ProtocolIdDataTransfer74 = 0x41;
        public const byte ProtocolIdDataTransfer266 = 0x42;
        public const byte ProtocolIdDataTransfer2058 = 0x43;
        public const byte ProtocolIdRegisterOper = 0xA0;
        public const byte ProtocolIdRegisterReply = 0xA8;

        /// <summary>
        /// 预设协议长度
        /// </summary>
        /// <param name="ProtocolId"></param>
        /// <returns></returns>
        public static int GetPacketLength(byte protocolId)
        {
            return protocolId switch
            {

                ProtocolIdAsciiTransfer64 => 64,
                ProtocolIdAsciiTransfer256 => 256,
                ProtocolIdAsciiTransfer2048 => 2048,
                ProtocolIdDataTransfer74 => 64 + 10,
                ProtocolIdDataTransfer266 => 256 + 10,
                ProtocolIdDataTransfer2058 => 2048 + 10,
                ProtocolIdRegisterOper => 256,
                ProtocolIdRegisterReply => 256,
                _ => -1,
            };
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
            }


            CarrotDataProtocolPacket pkt = new(buffer.Slice(buffer.Start, packetLen).ToArray());
            buffer = buffer.Slice(buffer.GetPosition(packetLen));
            packetsList.Add(pkt);
            //}

            packets = packetsList;
            return packetsList.Count != 0;
        }

        public override Packet Encode(byte[] payload)
        {
            return new CarrotDataProtocolPacket(payload);
        }

    }
}
