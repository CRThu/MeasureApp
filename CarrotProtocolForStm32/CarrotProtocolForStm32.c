
#include "./Protocol/Inc/payload_parse.h"

void test(const char* str, const char* paramtypes)
{
	payload_parse_t buf;
	payload_parse_init(&buf, (uint8_t*)str, strlen(str));

	printf("CMD: %s\n", str);

	int len1;
	char* str1;
	uint32_t num;
	double numf;
	for (int i = 0; i < strlen(paramtypes); i++)
	{
		switch (paramtypes[i])
		{
		case 's':
			str1 = (char*)malloc(256);
			memset(str1, 0, 256);
			len1 = payload_parse_string(&buf, str1, 256);
			printf("PAR%d: %s \t( len= %d )\n", i, str1, len1);
			free(str1);
			break;
		case 'i':
			num = payload_parse_int32(&buf);
			printf("PAR%d: %ld\n", i, num);
			break;
		case 'n':
			num = payload_parse_int32_dec(&buf);
			printf("PAR%d: %ld\n", i, num);
			break;
		case 'u':
			num = payload_parse_uint32(&buf);
			printf("PAR%d: %lu\n", i, num);
			break;
		case 'd':
			num = payload_parse_uint32_dec(&buf);
			printf("PAR%d: %lu\n", i, num);
			break;
		case 'h':
			num = payload_parse_uint32_hex(&buf);
			printf("PAR%d: 0x%X\n", i, num);
			break;
		case 'f':
			numf = payload_parse_double(&buf);
			printf("PAR%d: %.6lf\n", i, numf);
			break;
		}
	}
	printf("\n");
}

int main()
{
	printf("payload_parse_string(...)\n");
	test(" A  BB CCC  123 456", "sssssss");
	printf("payload_parse_int32(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "iiiiiiiiii");
	printf("payload_parse_int32_dec(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "nnnnnnnnnn");
	printf("payload_parse_uint32(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "uuuuuuuuuu");
	printf("payload_parse_uint32_dec(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "dddddddddd");
	printf("payload_parse_uint32_hex(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "hhhhhhhhhh");
	printf("payload_parse_double(...)\n");
	test(" 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010", "ffffffffff");
	printf("TEST CMD\n");
	test("SET CH 0 CODE 0x7FFF CH 1 VOLT 3.33333", "ssdsususf");
}
