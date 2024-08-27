#include <inttypes.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include "dynamic_pool.h"
#include "../Inc/bytes.h"

#ifndef CMD_PARSE_H
#define CMD_PARSE_H

#ifdef __cplusplus
extern "C"
{
#endif

#define CMD_PARSE_ELEMENT_DELIMITER(c)		(c == '(' || c == ',' || c == ')' || c == ';' )
#define CMD_PARSE_CHAR_IGNORE(c)			(c == ' ' || c == '\t' || c == '\r')
#define CMD_PARSE_END(c)					(c == '\n')

	typedef int8_t cmd_parse_status_t;
	/*
		funa(para,parb,parc,pard);
		reta=funb();
	*/

	cmd_parse_status_t cmd_parse_one(dynamic_pool_t* obj, callback_t** methods, uint16_t methods_count, char* cmd, uint16_t len);


#ifdef __cplusplus
}
#endif

#endif /* CMD_PARSE_H */