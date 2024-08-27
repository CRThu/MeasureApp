#pragma once
#include <inttypes.h>
#include <stdio.h>
#include "../Inc/cmd_parse.h"
#include "../Inc/dynamic_pool.h"

#define DYNAMIC_CALL_FUNC_MAX_CNT 256
#define DYNAMIC_CALL_ARGS_MAX_CNT 9

#ifndef DYNAMIC_CALL_H
#define DYNAMIC_CALL_H

#define FN_NAME_ISEQUAL(a,b)	(strcmp(a, b) == 0)
#define FN_ARGS_CNT(args)		(strlen(args))

#ifdef __cplusplus
extern "C"
{
#endif

	typedef void* callback;
	typedef void (*callback_a0r0)(void);
	typedef void (*callback_a1r0)(void* arg1);
	typedef void (*callback_a2r0)(void* arg1, void* arg2);
	typedef void (*callback_a3r0)(void* arg1, void* arg2, void* arg3);
	typedef void (*callback_a4r0)(void* arg1, void* arg2, void* arg3, void* arg4);
	typedef void (*callback_a5r0)(void* arg1, void* arg2, void* arg3, void* arg4, void* arg5);
	typedef void (*callback_a6r0)(void* arg1, void* arg2, void* arg3, void* arg4, void* arg5, void* arg6);
	typedef void (*callback_a7r0)(void* arg1, void* arg2, void* arg3, void* arg4, void* arg5, void* arg6, void* arg7);
	typedef void (*callback_a8r0)(void* arg1, void* arg2, void* arg3, void* arg4, void* arg5, void* arg6, void* arg7, void* arg8);
	typedef void (*callback_a9r0)(void* arg1, void* arg2, void* arg3, void* arg4, void* arg5, void* arg6, void* arg7, void* arg8, void* arg9);

	typedef struct {
		const char* name;
		callback func;
		const char* args;
	}callback_t;

	extern callback_t callbacks[];
	extern uint16_t callbacks_count;


	void dynamic_register(callback fn, char* name, char* args);
	void dynamic_call(payload_parse_t* args);


#ifdef __cplusplus
}
#endif

#endif /* DYNAMIC_CALL_H */