#include <inttypes.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#ifndef CMD_PARSE_H
#define CMD_PARSE_H

#ifdef __cplusplus
extern "C"
{
#endif

#define PAYLOAD_CHECK_SPACE(c) (c == ' ' || c == '\r' || c == '\n')

	typedef struct {
		/// <summary>
		/// 长度应小于32767
		/// </summary>
		uint8_t* buffer;
		uint16_t length;
		uint16_t cursor;
	}payload_parse_t;

	void payload_parse_init(payload_parse_t* buffer, uint8_t* payload, uint16_t len);

	uint16_t payload_parse_string(payload_parse_t* buffer, char* buf, uint16_t len);
	uint32_t payload_parse_uint32(payload_parse_t* buffer);
	uint32_t payload_parse_uint32_dec(payload_parse_t* buffer);
	uint32_t payload_parse_uint32_hex(payload_parse_t* buffer);
	int32_t payload_parse_int32(payload_parse_t* buffer);
	int32_t payload_parse_int32_dec(payload_parse_t* buffer);
	double payload_parse_double(payload_parse_t* buffer);


#ifdef __cplusplus
}
#endif

#endif /* CMD_PARSE_H */