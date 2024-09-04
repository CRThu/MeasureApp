#pragma once
#include <inttypes.h>
#include <stdio.h>
#include "dynamic_call.h"

#ifdef __cplusplus
extern "C"
{
#endif

#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif // !_CRT_SECURE_NO_WARNINGS

    extern char dynamic_call_buf[];

    void func_noargs_noreturn();
    void func_1args_dec64_noreturn(dyn_dec64p_t a);
    void func_1args_hex64_noreturn(dyn_hex64p_t a);
    void func_1args_string_noreturn(dyn_string_t a);
    //void func_1args_string_noreturn(double* a);
//void addi(int32_t* a, int32_t* b);
//void addf(double* a, double* b);

    void dynamic_call_register();
    void dynamic_call_print();



#ifdef __cplusplus
}
#endif