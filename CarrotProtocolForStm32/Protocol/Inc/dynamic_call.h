#pragma once
#include <inttypes.h>
#include <stdio.h>

#ifndef DYNAMIC_CALL_H
#define DYNAMIC_CALL_H

#ifdef __cplusplus
extern "C"
{
#endif

	typedef void(*callback);
	typedef void (*callback_a0r0)(void);
	typedef void (*callback_a1r0)(void* arg1);
	typedef void (*callback_a2r0)(void* arg1, void* arg2);

	typedef struct {
		const char* name;
		callback func;
		const char* args;
	}callback_t;

	void fnvoid();
	void pow(double a);
	void add(double a, double b);

	callback_t callbacks[] = {
		{"test", fnvoid, ""},
		{"pow", pow, "f"},
		{"add", add, "ff"}
	};


#ifdef __cplusplus
}
#endif

#endif /* DYNAMIC_CALL_H */