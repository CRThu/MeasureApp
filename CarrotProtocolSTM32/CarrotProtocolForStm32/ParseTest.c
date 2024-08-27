#include "./Protocol/Inc/cmd_parse.h"
#include "ParseTest.h"


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