#include "carrot_protocol.h"
#include <string.h>
#include <stdio.h>
#include <stdarg.h>

/*
USAGE:

1. PRINT STRING
        uint8_t txdata[256] = {0};
        carrot_data_protocol_256 *txp = (carrot_data_protocol_256 *)txdata;

        uint8_t msgbuf[80];
        uint8_t msglen = 0;
        msglen = sprintf((char *)msgbuf, "[set_data_protocol_256]: TIMER CALLBACK, Tick = %u\n", HAL_GetTick());
        set_data_protocol_256(txp, 0, 0, 0, msgbuf, msglen);
        uart_protocol_send(txp);


2. PRINT STRING
        uint8_t txdata[256] = {0};
        carrot_data_protocol_256 *txp = (carrot_data_protocol_256 *)txdata;

        protocol_256_print(txp, 0, 0, "[protocol_256_print]:1 TIMER CALLBACK, Tick = %u\n", HAL_GetTick());
        uart_protocol_send(txp);

3. RECEIVE DATA
        if(uart_protocol_available())
        {
            carrot_data_protocol_256 pkt;
            uart_protocol_receive((uint8_t*)&pkt);
            uart_protocol_send((uint8_t*)&pkt);
        }
*/

#define CARROT_DATA_PROTOCOL_FUNC_GEN(len)                                                                                                           \
                                                                                                                                                     \
    void gen_data_protocol_##len(carrot_data_protocol_##len *dat, uint16_t flag, uint8_t stream_id)                                                  \
    {                                                                                                                                                \
        dat->frame_start = CARROT_PROTOCOL_FRAME_START;                                                                                              \
        dat->protocol_id = CARROT_PROTOCOL_ID_DATA_##len;                                                                                            \
        dat->control_flags = flag;                                                                                                                   \
        dat->stream_id = stream_id;                                                                                                                  \
        dat->payload_len = 0;                                                                                                                        \
        memset(dat->payload, 0, sizeof(uint8_t) * (len - CARROT_PROTOCOL_DATA_PKG_BYTES));                                                           \
        dat->crc = 0;                                                                                                                                \
        dat->frame_end = CARROT_PROTOCOL_FRAME_END;                                                                                                  \
    }                                                                                                                                                \
                                                                                                                                                     \
    void set_data_protocol_##len(carrot_data_protocol_##len *dat, uint16_t flag, uint8_t stream_id, uint8_t verify, uint8_t *payload, uint16_t size) \
    {                                                                                                                                                \
        if (size > len - CARROT_PROTOCOL_DATA_PKG_BYTES)                                                                                             \
            size = len - CARROT_PROTOCOL_DATA_PKG_BYTES;                                                                                             \
                                                                                                                                                     \
        dat->frame_start = CARROT_PROTOCOL_FRAME_START;                                                                                              \
        dat->protocol_id = CARROT_PROTOCOL_ID_DATA_##len;                                                                                            \
        dat->control_flags = flag;                                                                                                                   \
        dat->stream_id = stream_id;                                                                                                                  \
        dat->payload_len = size;                                                                                                                     \
        memset(dat->payload, 0, sizeof(uint8_t) * (len - CARROT_PROTOCOL_DATA_PKG_BYTES));                                                           \
        memcpy(dat->payload, payload, sizeof(uint8_t) * size);                                                                                       \
        dat->crc = 0;                                                                                                                                \
        dat->frame_end = CARROT_PROTOCOL_FRAME_END;                                                                                                  \
                                                                                                                                                     \
        if (verify)                                                                                                                                  \
        {                                                                                                                                            \
            dat->crc = CRC_VERIFY_FUNC(dat, len);                                                                                                    \
        }                                                                                                                                            \
    }                                                                                                                                                \
                                                                                                                                                     \
    void protocol_##len##_print(carrot_data_protocol_##len *dat, uint16_t flag, uint8_t verify, const char *format, ...)                             \
    {                                                                                                                                                \
        va_list args;                                                                                                                                \
        uint32_t length;                                                                                                                             \
                                                                                                                                                     \
        gen_data_protocol_##len(dat, flag, CARROT_PROTOCOL_CONTROL_STREAM_ID);                                                                       \
                                                                                                                                                     \
        va_start(args, format);                                                                                                                      \
        length = vsnprintf((char *)dat->payload, sizeof(uint8_t) * (len - CARROT_PROTOCOL_DATA_PKG_BYTES), (char *)format, args);                    \
        va_end(args);                                                                                                                                \
                                                                                                                                                     \
        dat->payload_len = length;                                                                                                                   \
                                                                                                                                                     \
        if (verify)                                                                                                                                  \
        {                                                                                                                                            \
            dat->crc = CRC_VERIFY_FUNC(dat, len);                                                                                                    \
        }                                                                                                                                            \
    }

CARROT_DATA_PROTOCOL_FUNC_GEN(256);
CARROT_DATA_PROTOCOL_FUNC_GEN(2048);