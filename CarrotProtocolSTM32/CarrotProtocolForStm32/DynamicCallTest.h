#pragma once
#include <inttypes.h>
#include <stdio.h>
#include "dynamic_call.h"

#ifdef __cplusplus
extern "C"
{
#endif

	extern char dynamic_call_buf[];

	void func_noargs_noreturn();
	void func_1args_dec32_noreturn(int32_t* a);
	void func_1args_string_noreturn(char* a);
	//void func_1args_string_noreturn(double* a);
	//void addi(int32_t* a, int32_t* b);
	//void addf(double* a, double* b);

	void dynamic_call_register();
	void dynamic_call_print();



#ifdef __cplusplus
}
#endif#pragma once
