// carrot_protocol_util.h

#ifndef _CARROT_PROTOCOL_UTIL_H_
#define _CARROT_PROTOCOL_UTIL_H_

#include <stdint.h>
#include <ctype.h>
#include <string.h>

uint8_t str_split(char* haystack, char* needle, char** strs, int strs_len, char* defaultparval);
int hex2dec(char *hex, uint8_t len);

#endif // _CARROT_PROTOCOL_UTIL_H_
