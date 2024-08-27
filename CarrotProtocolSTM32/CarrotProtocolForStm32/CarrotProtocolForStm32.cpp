
#include "./Protocol/Inc/payload_parse.h"
#include "./Protocol/Inc/dynamic_call.h"
#include "ParseTest.h"
#include "DynamicCallTest.h"

int main()
{
	/*
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
	*/
	
	printf("TEST DYNAMIC CALL\n");
	dyn_reg_test();

	cmd_parse_t buf;
	const char* cmd0 = "print";
	payload_parse_init(&buf, (uint8_t*)cmd0, strlen(cmd0));
	dynamic_call(&buf);

	const char* cmd1 = "printi 12345";
	payload_parse_init(&buf, (uint8_t*)cmd1, strlen(cmd1));
	dynamic_call(&buf);

	const char* cmd2 = "prints helloworld";
	payload_parse_init(&buf, (uint8_t*)cmd2, strlen(cmd2));
	dynamic_call(&buf);

	const char* cmd3 = "addi 123 -456";
	payload_parse_init(&buf, (uint8_t*)cmd3, strlen(cmd3));
	dynamic_call(&buf);

	const char* cmd4 = "addf 123 -456";
	payload_parse_init(&buf, (uint8_t*)cmd4, strlen(cmd4));
	dynamic_call(&buf);
}
