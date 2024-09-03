#pragma once
#include <inttypes.h>
#include <stdio.h>
#include "../Inc/bytes.h"
#include "../Inc/dynamic_pool.h"

#ifndef DYNAMIC_CALL_H
#define DYNAMIC_CALL_H

#define DYNAMIC_CALL_VERSION    "1.0.0"

#define DYNAMIC_CALL_FUNC_MAX_CNT 256
#define DYNAMIC_CALL_ARGS_MAX_CNT 9

#define NAME_ISEQUAL(a,b)		(strcmp(a, b) == 0)
#define FN_ARGS_CNT(args)		(strlen(args))

#ifdef __cplusplus
extern "C"
{
#endif


    typedef int64_t*     dyn_dec64p_t;
    typedef uint64_t*    dyn_hex64p_t;
    typedef uint64_t*    dyn_enump_t;
    typedef char*       dyn_string_t;
    typedef uint8_t*    dyn_bytes_t;

    #define PVAL(p)      (*(p))


    typedef void* delegate;
    typedef void (*delegate_a0r0)(void);
    typedef void (*delegate_a1r0)(void* arg1);
    typedef void (*delegate_a2r0)(void* arg1, void* arg2);
    typedef void (*delegate_a3r0)(void* arg1, void* arg2, void* arg3);
    typedef void (*delegate_a4r0)(void* arg1, void* arg2, void* arg3, void* arg4);
    typedef void (*delegate_a5r0)(void* arg1, void* arg2, void* arg3, void* arg4, void* arg5);
    typedef void (*delegate_a6r0)(void* arg1, void* arg2, void* arg3, void* arg4, void* arg5, void* arg6);
    typedef void (*delegate_a7r0)(void* arg1, void* arg2, void* arg3, void* arg4, void* arg5, void* arg6, void* arg7);
    typedef void (*delegate_a8r0)(void* arg1, void* arg2, void* arg3, void* arg4, void* arg5, void* arg6, void* arg7, void* arg8);
    typedef void (*delegate_a9r0)(void* arg1, void* arg2, void* arg3, void* arg4, void* arg5, void* arg6, void* arg7, void* arg8, void* arg9);

    typedef struct {
        char* name;
        delegate handler;
        dtypes_t* args_type;
        uint8_t args_count;
    }delegate_t;

    extern delegate_t delegates[];
    extern uint16_t delegates_count;


    void delegate_register(delegate delegate, char* name, dtypes_t* args_type, uint8_t args_count);
    delegate_t* find_delegate_by_name(delegate_t* delegates, uint16_t delegates_count, char* name);
    void invoke(dynamic_pool_t* pool, delegate_t* f);


#ifdef __cplusplus
}
#endif

#endif /* DYNAMIC_CALL_H */