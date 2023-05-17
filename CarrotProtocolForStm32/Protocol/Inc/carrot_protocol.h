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

#define CARROT_PROTOCOL_CONTROL_STREAM_ID 0
#define CARROT_PROTOCOL_DATA0_STREAM_ID 1

#define CARROT_PROTOCOL_ACK_RESERVED_LEN 5
#define CARROT_PROTOCOL_DATA_PKG_BYTES 10
#define CARROT_PROTOCOL_DATA_64_PAYLOAD_MAX_LEN (64 - CARROT_PROTOCOL_DATA_PKG_BYTES)
#define CARROT_PROTOCOL_DATA_256_PAYLOAD_MAX_LEN (256 - CARROT_PROTOCOL_DATA_PKG_BYTES)
#define CARROT_PROTOCOL_DATA_2048_PAYLOAD_MAX_LEN (2048 - CARROT_PROTOCOL_DATA_PKG_BYTES)

#define CARROT_PROTOCOL_CRC 1

#if CARROT_PROTOCOL_CRC == 0
#define CRC_VERIFY_FUNC(ADDR, LEN) (0x0000)
#elif CARROT_PROTOCOL_CRC == 1
// HARD CRC CALC
// STM32 F1/F4/L1 DEFAULT: CRC32
// Uses CRC-32 (Ethernet) polynomial: 0x4C11DB7
// X32 + X26 + X23 + X22 + X16 + X12 + X11 + X10 + X8 + X7 + X5 + X4 + X2 + X + 1
// Single input/output 32-bit data register
// reset to 0xFFFF FFFF
// use whole packet with crc16=0x0000 to calc crc32 and get low 16bit
#include "crc.h"
#define CRC_PROTOCOL_INSTANCE hcrc
#define CRC_VERIFY_FUNC(ADDR, LEN) (uint16_t) HAL_CRC_Calculate(&CRC_PROTOCOL_INSTANCE, (uint32_t *)(ADDR), (uint32_t)((LEN) / 4))
#endif

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

#define CARROT_DATA_PROTOCOL_GEN(len)                                                                                                                 \
	typedef struct __PROTOCOL_PACKED__                                                                                                                \
	{                                                                                                                                                 \
		uint8_t frame_start;                                                                                                                          \
		uint8_t protocol_id;                                                                                                                          \
		uint16_t control_flags;                                                                                                                       \
		uint8_t stream_id;                                                                                                                            \
		uint16_t payload_len;                                                                                                                         \
		uint8_t payload[len - CARROT_PROTOCOL_DATA_PKG_BYTES];                                                                                        \
		uint16_t crc;                                                                                                                                 \
		uint8_t frame_end;                                                                                                                            \
	} carrot_data_protocol_##len;                                                                                                                     \
                                                                                                                                                      \
	void gen_data_protocol_##len(carrot_data_protocol_##len *dat, uint16_t flag, uint8_t stream_id);                                                  \
	void set_data_protocol_##len(carrot_data_protocol_##len *dat, uint16_t flag, uint8_t stream_id, uint8_t verify, uint8_t *payload, uint16_t size); \
	void protocol_##len##_print(carrot_data_protocol_##len *dat, uint16_t flag, uint8_t verify, const char *format, ...)

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

#ifdef __cplusplus
}
#endif

#endif /* CARROT_PROTOCOL_H */
