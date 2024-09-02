#define _CRT_SECURE_NO_WARNINGS

#include "DynamicCallTest.h"

char dynamic_call_buf[256];

void func_noargs_noreturn()
{
	sprintf(dynamic_call_buf, "func_noargs_noreturn() Called.");
	puts(dynamic_call_buf);
}

void func_1args_dec64_noreturn(dyn_dec64_t a)
{
	sprintf(dynamic_call_buf, "func_1args_dec64_noreturn(%"PRId64") Called.", a);
	puts(dynamic_call_buf);
}

void func_1args_hex64_noreturn(dyn_hex64_t a)
{
	sprintf(dynamic_call_buf, "func_1args_hex64_noreturn(0x%"PRIX64") Called.", a);
	puts(dynamic_call_buf);
}

void func_1args_string_noreturn(dyn_string_t a)
{
	sprintf(dynamic_call_buf, "func_1args_string_noreturn(%s) Called.", a);
	puts(dynamic_call_buf);
}

/*
void addi(int32_t* a, int32_t* b)
{
	sprintf(dynamic_call_test_buf, "addi(%d, %d) Called.", *a, *b);
	puts(dynamic_call_test_buf);
}

void addf(double* a, double* b)
{
	sprintf(dynamic_call_test_buf, "addf(%f, %f) Called.", *a, *b);
	puts(dynamic_call_test_buf);
}
*/

void dynamic_call_register()
{
	delegate_register(func_noargs_noreturn, "func_noargs_noreturn", NULL, 0);
	dtypes_t func_1args_dec64_noreturn_argtype[1] = { T_DEC64 };
	delegate_register(func_1args_dec64_noreturn, "func_1args_dec64_noreturn", func_1args_dec64_noreturn_argtype, 1);
	dtypes_t func_1args_hex64_noreturn_argtype[1] = { T_DEC64 };
	delegate_register(func_1args_hex64_noreturn, "func_1args_hex64_noreturn", func_1args_hex64_noreturn_argtype, 1);
	dtypes_t func_1args_string_noreturn_argtype[1] = { T_STRING };
	delegate_register(func_1args_string_noreturn, "func_1args_string_noreturn", func_1args_string_noreturn_argtype, 1);
	/*
	delegate_register(prints, "prints", "s");
	delegate_register(addi, "addi", "ii");
	delegate_register(addf, "addf", "ff");
	*/
}

void dynamic_call_print()
{

	printf("%s\n%s\n%s\n", __FUNCTION__, __FUNCDNAME__, __FUNCSIG__);

}