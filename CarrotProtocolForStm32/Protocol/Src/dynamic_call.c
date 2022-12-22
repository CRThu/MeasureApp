#include "../Inc/dynamic_call.h"
#include "../Inc/payload_parse.h"

void fnvoid()
{
	printf("fnvoid() Called.\r\n");
}

void pow(double a)
{
	printf("pow(double) Called.\r\n");
}

void add(double a, double b)
{
	printf("add(double, double) Called.\r\n");
}

void dynamic_call(char* fn_name, payload_parse_t args)
{
	void* par[8];
	for (int i = 0; i < sizeof(callbacks) / sizeof(callbacks[0]); i++)
	{
		if (!strcmp(callbacks[i].name, fn_name))
		{
			// TODO
		}
	}
}