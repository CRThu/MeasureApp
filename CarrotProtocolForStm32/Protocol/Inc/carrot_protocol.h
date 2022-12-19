#include <inttypes.h>

#ifndef CARROT_PROTOCOL_H
#define CARROT_PROTOCOL_H

#ifdef __cplusplus
extern "C"
{
#endif

#define CARROT_PROTOCOL_FRAME_START 0x3C
#define CARROT_PROTOCOL_FRAME_END 0x3E
#define CARROT_PROTOCOL_ID_ACK 0x30
#define CARROT_PROTOCOL_ID_DATA_64 0x31
#define CARROT_PROTOCOL_ID_DATA_256 0x32
#define CARROT_PROTOCOL_ID_DATA_2048 0x33

#define CARROT_PROTOCOL_ACK_RESERVED_LEN 5
#define CARROT_PROTOCOL_DATA_PKG_BYTES 10
#define CARROT_PROTOCOL_DATA_64_PAYLOAD_MAX_LEN (64 - CARROT_PROTOCOL_DATA_PKG_BYTES)
#define CARROT_PROTOCOL_DATA_256_PAYLOAD_MAX_LEN (256 - CARROT_PROTOCOL_DATA_PKG_BYTES)
#define CARROT_PROTOCOL_DATA_2048_PAYLOAD_MAX_LEN (2048 - CARROT_PROTOCOL_DATA_PKG_BYTES)

	// KEIL ARMCC and GNUC
#if defined(__ARMCC_VERSION) && (__ARMCC_VERSION >= 6010050) /* ARM Compiler V6 */
#ifndef __PROTOCOL_PACKED__
#define __PROTOCOL_PACKED__ __attribute__((packed))
#endif
#elif defined(__GNUC__) && !defined(__CC_ARM) /* GNU Compiler */
#ifndef __PROTOCOL_PACKED__
#define __PROTOCOL_PACKED__ __attribute__((__packed__))
#endif /* __PROTOCOL_PACKED__ */
#else
#ifndef __PROTOCOL_PACKED__
#define __PROTOCOL_PACKED__
#endif /* __PROTOCOL_PACKED__ */
#endif /* __GNUC__ */

// ALIGN START
#if defined(_MSC_VER)
#pragma pack(push, 1)
#endif

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
	} carrot_data_protocol_##len;                                  \
                                                                   \
	void gen_data_protocol_##len(carrot_data_protocol_##len *dat); \
	void set_data_protocol_##len(carrot_data_protocol_##len *dat, uint8_t stream_id, uint8_t *payload, uint16_t size, uint8_t verify);\

	CARROT_DATA_PROTOCOL_GEN(64);
	CARROT_DATA_PROTOCOL_GEN(256);
	CARROT_DATA_PROTOCOL_GEN(2048);

	typedef struct __PROTOCOL_PACKED__
	{
		uint8_t frame_start;
		uint8_t protocol_id;
		uint8_t reserved[CARROT_PROTOCOL_ACK_RESERVED_LEN];
		uint8_t frame_end;
	} carrot_ack_protocol;

	// ALIGN END
#if defined(_MSC_VER)
#pragma pack(pop)
#endif

	void gen_ack_protocol(carrot_ack_protocol *ack);

#ifdef __cplusplus
}
#endif

#endif /* CARROT_PROTOCOL_H */
