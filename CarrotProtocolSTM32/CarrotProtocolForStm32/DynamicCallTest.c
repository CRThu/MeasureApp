#define _CRT_SECURE_NO_WARNINGS

#include "DynamicCallTest.h"

char dynamic_call_buf[256];

void func_noargs_noreturn()
{
	sprintf(dynamic_call_buf, "print() Called.");
	puts(dynamic_call_buf);
}

void func_1args_dec32_noreturn(int32_t* a)
{
	sprintf(dynamic_call_buf, "printi(%d) Called.", *a);
	puts(dynamic_call_buf);
}

void print_1args_string_noreturn(char* a)
{
	sprintf(dynamic_call_buf, "prints(%s) Called.", a);
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
	dynamic_register(func_noargs_noreturn, "func_noargs_noreturn", "");
	dynamic_register(func_1args_dec32_noreturn, "func_1args_dec32_noreturn", "d");
	dynamic_register(print_1args_string_noreturn, "print_1args_string_noreturn", "f");
	/*
	dynamic_register(prints, "prints", "s");
	dynamic_register(addi, "addi", "ii");
	dynamic_register(addf, "addf", "ff");
	*/
}