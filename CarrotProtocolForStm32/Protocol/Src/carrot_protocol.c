#include "carrot_protocol.h"
#include <string.h>

#define CARROT_DATA_PROTOCOL_FUNC_GEN(len)   \
\
void gen_data_protocol_##len(carrot_data_protocol_##len* dat)\
{\
    dat->frame_start = CARROT_PROTOCOL_FRAME_START;\
    dat->protocol_id = CARROT_PROTOCOL_ID_DATA_##len;\
    dat->control_flags = 0;\
    dat->stream_id = 0;\
    dat->payload_len = 0;\
    memset(dat->payload, 0, sizeof(uint8_t) * (len - CARROT_PROTOCOL_DATA_PKG_BYTES));\
    dat->crc16 = 0;\
    dat->frame_end = CARROT_PROTOCOL_FRAME_END;\
}\
\
void set_data_protocol_##len(carrot_data_protocol_##len* dat, uint8_t stream_id, uint8_t* payload, uint16_t size, uint8_t verify)\
{\
    dat->stream_id = 0;\
    dat->payload_len = size;\
    memcpy(dat->payload, payload, sizeof(uint8_t) * size);\
    if (verify)\
    {\
    }\
}\

CARROT_DATA_PROTOCOL_FUNC_GEN(64);
CARROT_DATA_PROTOCOL_FUNC_GEN(256);
CARROT_DATA_PROTOCOL_FUNC_GEN(2048);

void gen_ack_protocol(carrot_ack_protocol* ack)
{
	ack->frame_start = CARROT_PROTOCOL_FRAME_START;
	ack->protocol_id = CARROT_PROTOCOL_ID_ACK;
	memset(ack->reserved, 0, sizeof(uint8_t) * CARROT_PROTOCOL_ACK_RESERVED_LEN);
	ack->frame_end = CARROT_PROTOCOL_FRAME_END;
}
