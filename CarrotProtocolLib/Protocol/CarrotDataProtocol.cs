using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CarrotProtocolLib.Device;
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

    public class CarrotDataProtocol : IProtocol
    {
        public IDevice Device { get; set; }
        public ILogger Logger { get; set; }
        private CancellationTokenSource ParseProtocolTaskCts { get; set; }
        private Task<int> ParseProtocolTask { get; set; }


        //public delegate void ProtocolParseErrorHandler(Exception ex);
        //public event ProtocolParseErrorHandler ProtocolParseError;
        public event IProtocol.ProtocolParseErrorHandler ProtocolParseError;

        public CarrotDataProtocol(IDevice device, ILogger logger)
        {
            Device = device;
            Logger = logger;
        }

        public CarrotDataProtocol(IDevice device, ILogger logger, IProtocol.ProtocolParseErrorHandler protocolParseErrorHandler) : this(device, logger)
        {
            ProtocolParseError += protocolParseErrorHandler;
        }

        public void Start()
        {
            Device.Open();
            ParseProtocolTaskCts = new();
            ParseProtocolTask = Task.Run(() => ParseProtocolLoop(), ParseProtocolTaskCts.Token);
        }

        public void Stop()
        {
            Device.Close();
            ParseProtocolTaskCts.Cancel();
            ParseProtocolTask.Wait();
        }

        private int ParseProtocolLoop()
        {
            try
            {
                byte[] frame = new byte[65536];
                int frameCursor = 0;
                int readBytes = 0;
                int pktLength = 0;
                int waitBytesNum = 0;
                DataProtocolParseState state = DataProtocolParseState.WAIT_FRAME_START;
                while (true)
                {
                    // 计算触发协议读取长度
                    switch (state)
                    {
                        case DataProtocolParseState.WAIT_FRAME_START:
                        case DataProtocolParseState.WAIT_PROTOCOL_ID:
                        case DataProtocolParseState.WAIT_FRAME_END:
                            waitBytesNum = 1;
                            break;
                        case DataProtocolParseState.WAIT_PROTOCOL_DATA:
                            waitBytesNum = pktLength - 3;
                            break;
                    }

                    // 数据不满足长度要求则等待
                    if (Device.RxByteToRead < waitBytesNum)
                    {
                        Thread.Sleep(10);
                        //cts.Token.ThrowIfCancellationRequested();
                        if (ParseProtocolTaskCts.Token.IsCancellationRequested)
                            return 0;
                    }
                    else
                    {
                        // 帧数据处理状态机
                        switch (state)
                        {
                            case DataProtocolParseState.WAIT_FRAME_START:
                                frameCursor = 0;
                                readBytes = 1;
                                Device.Read(frame, frameCursor, readBytes);
                                if (frame[frameCursor] == CarrotDataProtocolRecord.FrameStartByte)
                                {
                                    state = DataProtocolParseState.WAIT_PROTOCOL_ID;
                                    frameCursor += readBytes;
                                }
                                else
                                {
                                    state = DataProtocolParseState.WAIT_FRAME_START;
                                }
                                break;
                            case DataProtocolParseState.WAIT_PROTOCOL_ID:
                                readBytes = 1;
                                Device.Read(frame, frameCursor, readBytes);
                                pktLength = CarrotDataProtocolRecord.GetPacketLength(frame[frameCursor]);
                                if (pktLength > 0)
                                {
                                    state = DataProtocolParseState.WAIT_PROTOCOL_DATA;
                                    frameCursor += readBytes;
                                }
                                else
                                {
                                    state = DataProtocolParseState.WAIT_PROTOCOL_ID;
                                }
                                break;
                            case DataProtocolParseState.WAIT_PROTOCOL_DATA:
                                readBytes = pktLength - 3;
                                Device.Read(frame, frameCursor, readBytes);
                                state = DataProtocolParseState.WAIT_FRAME_END;
                                frameCursor += readBytes;
                                break;
                            case DataProtocolParseState.WAIT_FRAME_END:
                                readBytes = 1;
                                Device.Read(frame, frameCursor, readBytes);
                                if (frame[frameCursor] == CarrotDataProtocolRecord.FrameEndByte)
                                {
                                    state = DataProtocolParseState.WAIT_FRAME_START;
                                    frameCursor += readBytes;
                                    AddLog(new CarrotDataProtocolRecord(frame, 0, frameCursor));
                                }
                                else
                                {
                                    state = DataProtocolParseState.WAIT_FRAME_END;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ProtocolParseError?.Invoke(ex);
                return -1;
            }
        }

        public void Send(IProtocolRecord protocol)
        {
            Send(protocol.Bytes);
        }

        public void Send(byte[] bytes)
        {
            Send(bytes, 0, bytes.Length);
        }

        public void Send(byte[] bytes, int offset, int length)
        {
            Device.Write(bytes);
            Logger.AddTx(new CarrotDataProtocolRecord(bytes, offset, length));
        }

        private void AddLog(CarrotDataProtocolRecord protocol)
        {
            Logger.AddRx(protocol);
        }

        private enum DataProtocolParseState
        {
            WAIT_FRAME_START,
            WAIT_PROTOCOL_ID,
            WAIT_PROTOCOL_DATA,
            WAIT_FRAME_END
        }
    }
}