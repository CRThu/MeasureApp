#define _CRT_SECURE_NO_WARNINGS

#include "DynamicCallTest.h"

char dynamic_call_test_buf[256];

void print()
{
	sprintf(dynamic_call_test_buf, "print() Called.");
	puts(dynamic_call_test_buf);
}

void printi(int32_t* a)
{
	sprintf(dynamic_call_test_buf, "printi(%d) Called.", *a);
	puts(dynamic_call_test_buf);
}

void printff(double* a)
{
	sprintf(dynamic_call_test_buf, "printff(%.6lf) Called.", *a);
	puts(dynamic_call_test_buf);
}

void prints(char* a)
{
	sprintf(dynamic_call_test_buf, "prints(%s) Called.", a);
	puts(dynamic_call_test_buf);
}

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

void dyn_reg_test()
{
	dynamic_register(print, "print", "");
	dynamic_register(printi, "printi", "i");
	dynamic_register(printff, "printff", "f");
	dynamic_register(prints, "prints", "s");
	dynamic_register(addi, "addi", "ii");
	dynamic_register(addf, "addf", "ff");
}