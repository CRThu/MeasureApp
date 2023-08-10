using CarrotProtocolLib.Device;
using CarrotProtocolLib.Protocol;
using CarrotProtocolLib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrotProtocolLib.Service
{
    /// <summary>
    /// CarrotDataProtocol协议解码
    /// </summary>
    public class CarrotDataProtocolDecodeService : ProtocolDecodeBaseService
    {

        /// <summary>
        /// 解码缓冲区(内部使用)
        /// </summary>
        private byte[] DecodeBuffer { get; set; }

        /// <summary>
        /// 解码缓冲区(内部使用)
        /// </summary>
        private int DecodeBufferCursor { get; set; }


        /// <summary>
        /// 构造函数
        /// </summary>
        public CarrotDataProtocolDecodeService() : base()
        {
            DecodeBuffer = new byte[64 * 1024];
            DecodeBufferCursor = 0;
        }

        /// <summary>
        /// 数据接收服务程序
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override int ServiceLoop()
        {
            CarrotProtocolFrameDecodeFsmState frameDecodeFsmState = CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_START;
            int requiredBytesToRead = 0;
            int frameLength = -1;

            while (true)
            {
                // 外部退出请求检测
                if (IsCancellationRequested)
                    return 1;

                // 获取缓冲区数据
                int len = Device!.RxByteToRead;

                // 计算协议读取长度
                switch (frameDecodeFsmState)
                {
                    case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_START:
                        // BYTES SUM        1
                        // ------------------
                        // FrameStart       1
                        requiredBytesToRead = 1;
                        break;
                    case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_PROTOCOL_ID:
                        // BYTES SUM        1
                        // ------------------
                        // ProtocolId       1
                        requiredBytesToRead = 1;
                        break;
                    case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_CTL_PREFIX:
                        // BYTES SUM        5
                        // ------------------
                        // ControlFlags     2
                        // StreamId         1
                        // PayloadLength    2
                        requiredBytesToRead = 5;
                        break;
                    case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_PAYLOAD:
                        // BYTES SUM        LEN-10
                        // -----------------------
                        // Payload          LEN-10
                        requiredBytesToRead = frameLength - 10;
                        break;
                    case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_CTL_SUFFIX:
                        // BYTES SUM        2
                        // ------------------
                        // Crc16            2
                        requiredBytesToRead = 2;
                        break;
                    case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_END:
                        // BYTES SUM        1
                        // ------------------
                        // FrameEnd         1
                        requiredBytesToRead = 1;
                        break;
                    default:
                        break;
                }

                if (len >= requiredBytesToRead)
                {
                    // 帧数据处理状态机
                    switch (frameDecodeFsmState)
                    {
                        case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_START:
                            // 读取帧起始字节
                            Device.Read(DecodeBuffer, DecodeBufferCursor, requiredBytesToRead);
                            if (DecodeBuffer[DecodeBufferCursor] == CarrotDataProtocolFrame.FrameStartByte)
                            {
                                frameDecodeFsmState = CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_PROTOCOL_ID;
                                DecodeBufferCursor += requiredBytesToRead;
                            }
                            else
                            {
                                // 数据帧起始字节错误
                                throw new NotImplementedException();
                            }
                            break;
                        case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_PROTOCOL_ID:
                            // 读取帧ID
                            Device.Read(DecodeBuffer, DecodeBufferCursor, requiredBytesToRead);
                            frameLength = CarrotDataProtocolFrame.GetPacketLength(DecodeBuffer[DecodeBufferCursor]);
                            if (frameLength > 0)
                            {
                                frameDecodeFsmState = CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_CTL_PREFIX;
                                DecodeBufferCursor += requiredBytesToRead;
                            }
                            else
                            {
                                // 数据帧协议ID错误
                                throw new NotImplementedException();
                            }
                            break;
                        case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_CTL_PREFIX:
                            // 读取前部控制流
                            Device.Read(DecodeBuffer, DecodeBufferCursor, requiredBytesToRead);
                            if (true)   // RESERVED FOR CONTROL BITS
                            {
                                frameDecodeFsmState = CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_PAYLOAD;
                                DecodeBufferCursor += requiredBytesToRead;
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                            break;
                        case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_PAYLOAD:
                            // 读取数据
                            Device.Read(DecodeBuffer, DecodeBufferCursor, requiredBytesToRead);
                            if (true)   // RESERVED FOR PAYLOAD
                            {
                                frameDecodeFsmState = CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_CTL_SUFFIX;
                                DecodeBufferCursor += requiredBytesToRead;
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                            break;
                        case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_CTL_SUFFIX:
                            // 读取后部控制流
                            Device.Read(DecodeBuffer, DecodeBufferCursor, requiredBytesToRead);
                            if (true)   // RESERVED FOR CONTROL BITS
                            {
                                frameDecodeFsmState = CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_END;
                                DecodeBufferCursor += requiredBytesToRead;
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                            break;
                        case CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_END:
                            // 读取帧结束字节
                            Device.Read(DecodeBuffer, DecodeBufferCursor, requiredBytesToRead);
                            if (DecodeBuffer[DecodeBufferCursor] == CarrotDataProtocolFrame.FrameEndByte)
                            {
                                frameDecodeFsmState = CarrotProtocolFrameDecodeFsmState.WAIT_FRAME_START;
                                DecodeBufferCursor += requiredBytesToRead;

                                if (DecodeBufferCursor != frameLength)
                                    // 判断数据长度解码错误
                                    throw new NotImplementedException();

                                Logger!.AddRx(new CarrotDataProtocolFrame(DecodeBuffer, 0, frameLength));

                                DecodeBufferCursor = 0;
                            }
                            else
                            {
                                // 数据帧结束字节错误
                                throw new NotImplementedException();
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
            }
            return 0;
        }
    }

    public enum CarrotProtocolFrameDecodeFsmState
    {
        WAIT_FRAME_START,
        WAIT_FRAME_PROTOCOL_ID,
        WAIT_FRAME_CTL_PREFIX,
        WAIT_FRAME_PAYLOAD,
        WAIT_FRAME_CTL_SUFFIX,
        WAIT_FRAME_END
    }
}
