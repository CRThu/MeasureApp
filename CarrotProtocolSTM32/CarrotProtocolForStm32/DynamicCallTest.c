#define _CRT_SECURE_NO_WARNINGS

#include "DynamicCallTest.h"

char dynamic_call_test_buf[256];

void print_0_0()
{
	sprintf(dynamic_call_test_buf, "print() Called.");
	puts(dynamic_call_test_buf);
}

void print_d_0(int32_t* a)
{
	sprintf(dynamic_call_test_buf, "printi(%d) Called.", *a);
	puts(dynamic_call_test_buf);
}

void print_s_0(char* a)
{
	sprintf(dynamic_call_test_buf, "prints(%s) Called.", a);
	puts(dynamic_call_test_buf);
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
	dynamic_register(print_0_0, "print_0_0", "");
	dynamic_register(print_d_0, "print_d_0", "d");
	dynamic_register(print_s_0, "print_s_0", "f");
	/*
	dynamic_register(prints, "prints", "s");
	dynamic_register(addi, "addi", "ii");
	dynamic_register(addf, "addf", "ff");
	*/
}