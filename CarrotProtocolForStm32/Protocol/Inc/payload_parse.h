#include <inttypes.h>
#include <string.h>

#ifndef PAYLOAD_PARSE_H
#define PAYLOAD_PARSE_H

#ifdef __cplusplus
extern "C"
{
#endif

#define PAYLOAD_CHECK_SPACE(c) (c == ' ' || c == '\r' || c == '\n')

	typedef struct payload_parse_t payload_parse_t;

	struct payload_parse_t {
		/// <summary>
		/// 长度应小于32767
		/// </summary>
		uint8_t* buffer;
		uint16_t length;
		uint16_t cursor;
	};

	void payload_parse_init(payload_parse_t* buffer, uint8_t* payload, uint16_t len);

	uint16_t payload_parse_string(payload_parse_t* buffer, char* buf, uint16_t len);
	uint16_t payload_parse_uint16(payload_parse_t* buffer);
	//uint16_t payload_parse_uint16_hex(payload_parse_t* buffer);
	//int16_t payload_parse_int16(payload_parse_t* buffer);
	//double payload_parse_double(payload_parse_t* buffer);


#ifdef __cplusplus
}
#endif

#endif /* PAYLOAD_PARSE_H */