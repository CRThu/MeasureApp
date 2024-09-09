/****************************
 * ASCII PROTOCOL
 * CARROT HU
 * 2024.09.05
 *****************************/
#pragma once

#ifndef _ASCII_PROTOCOL_H_
#define _ASCII_PROTOCOL_H_

#include <stdint.h>
#include <string.h>
#include <stdio.h>
#include <stdarg.h>
#include "uart_comm.h"

#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif // !_CRT_SECURE_NO_WARNINGS

#ifdef __cplusplus
extern "C"
{
#endif
#define ASCII_PROTOCOL_VERSION "1.0.2"

#define DEBUG_MSG()         protocol_write_msg();

    void protocol_parse(uint8_t* buf, uint16_t len, uint16_t* endpos);
    void protocol_write_msg(const char* format, ...);
    void protocol_write_data(uint8_t* data, uint16_t len);


#ifdef __cplusplus
}
#endif

#endif // _ASCII_PROTOCOL_H_