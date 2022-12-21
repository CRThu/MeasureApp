
#include "./Protocol/Inc/payload_parse.h"
#include "ParseTest.h"

int main()
{
	printf("payload_parse_string(...)\n");
	test(" A  BB CCC  123 456", "sssssss");
	printf("payload_parse_int32(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "iiiiiiiiiii");
	printf("payload_parse_int32_dec(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "nnnnnnnnnnn");
	printf("payload_parse_uint32(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "uuuuuuuuuuu");
	printf("payload_parse_uint32_dec(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "ddddddddddd");
	printf("payload_parse_uint32_hex(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "hhhhhhhhhhh");
	printf("payload_parse_double(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "fffffffffff");
	printf("TEST CMD\n");
	test("SET CH 0 CODE 0x7FFF CH 1 VOLT 3.33333", "ssdsususfs");
}
