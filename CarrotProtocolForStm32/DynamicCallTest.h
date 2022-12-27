#pragma once
#include <inttypes.h>
#include <stdio.h>
#include "Protocol/Inc/dynamic_call.h"

#ifdef __cplusplus
extern "C"
{
#endif

	extern char dynamic_call_test_buf[];

	void print();
	void printi(int32_t* a);
	void printff(double* a);
	void prints(char* a);
	void addi(int32_t* a, int32_t* b);
	void addf(double* a, double* b);

	void dyn_reg_test();



#ifdef __cplusplus
}
#endif#pragma once
