// carrot_protocol_util.c

#include "carrot_protocol_util.h"


/*
USAGE

    #define PAR_DEF_MAX 88
    
	char buf000[256] = "STAT;3;f;";
	char* haystack = buf000;
	char* needle = ";";
	char* strs[PAR_DEF_MAX] = { 0x00 };
	char* default_parval = "0";
	uint8_t strs_cnt = str_split(haystack, needle, strs, PAR_DEF_MAX, default_parval);

	printf("PARAM CNT=%d, \n", strs_cnt);
	for (int i = 0; i < PAR_DEF_MAX; i++)
		printf("%d: %s(COUNT=%d) \n", i, strs[i], strlen(strs[i]));

*/
uint8_t str_split(char* haystack, char* needle, char** strs, int strs_len, char* defaultparval)
{
	char* buf = strstr(haystack, needle);

	for (int i = 0; i < strs_len; i++)
		strs[i] = defaultparval;

	uint8_t cnt = 0;
	while (buf != NULL)
	{
		buf[0] = '\0';
		//usb_printf( "%s\n", haystack);
		strs[cnt] = haystack;
		haystack = buf + strlen(needle);
		buf = strstr(haystack, needle);
		cnt++;
	}
	return cnt;
}

int c2i(char ch)
{
	if (isdigit(ch))
		return ch - 48;

	if (ch < 'A' || (ch > 'F' && ch < 'a') || ch > 'z')
		return -1;

	if (isalpha(ch))
		return isupper(ch) ? ch - 55 : ch - 87;

	return -1;
}

int hex2dec(char *hex, uint8_t len)
{
	int num = 0;
	int temp;
	int bits;
	int i;
	for (i = 0, temp = 0; i < len; i++, temp = 0)
	{
		temp = c2i(*(hex + i));
		bits = (len - i - 1) * 4;
		temp = temp << bits;
		num = num | temp;
	}
	return num;
}