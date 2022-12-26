#include "DynamicCallTest.h"

void print()
{
	printf("print() Called.\r\n");
}

void printi(int32_t* a)
{
	printf("printi(%d) Called.\r\n", *a);
}

void printff(double* a)
{
	printf("printff(%.6lf) Called.\r\n", *a);
}

void prints(char* a)
{
	printf("printff(%s) Called.\r\n", a);
}

void addi(int32_t* a, int32_t* b)
{
	printf("add(%d, %d) Called.\r\n", *a, *b);
}

void addf(double* a, double* b)
{
	printf("addf(%f, %f) Called.\r\n", *a, *b);
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