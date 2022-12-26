#pragma once
#include <inttypes.h>
#include <stdio.h>
#include "../Inc/payload_parse.h"
#include "../Inc/dynamic_type_array.h"

#define DYNAMIC_CALL_FUNC_MAX_CNT 256

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