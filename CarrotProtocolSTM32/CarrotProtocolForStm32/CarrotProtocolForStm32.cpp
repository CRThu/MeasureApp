#include "cmd_parse.h"
#include "dynamic_call.h"
#include "DynamicCallTest.h"

void parse_test(dynamic_pool_t* pool)
{
	dynamic_pool_init(pool);

	uint8_t types[] = { T_STRING, T_DEC64, T_HEX64 };
	uint8_t types_len = sizeof(types);

	printf("TYPE: %02X,%02X,%02X\n", types[0], types[1], types[2]);

	char c[256] = "";
	char* p = c;
	strcpy(p, "fun_abc(32,10)\n");

	cmd_parse_one(pool, types, types_len, p, strlen(p));

	printf("CMD: %s\n", p);

	dynamic_pool_print(pool);

	printf("\n");
}

void invoke_test(dynamic_pool_t* pool)
{
	dynamic_call_register();

	char s[256];
	strcpy(s, "func_noargs_noreturn");

	invoke(pool, &(delegates[0]));
}

int main()
{
	dynamic_pool_t pool;
	parse_test(&pool);
	invoke_test(&pool);
}
