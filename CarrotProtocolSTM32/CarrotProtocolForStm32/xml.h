/****************************
* XML
* CARROT HU
* 2024.10.16
*****************************/
#pragma once
#include <inttypes.h>
#include <stdio.h>

#ifndef XML_H
#define XML_H

#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif // !_CRT_SECURE_NO_WARNINGS

#ifdef __cplusplus
extern "C"
{
#endif

#define XML_VERSION		"1.0.0"


    typedef struct {
        const uint8_t* buffer;
        uint16_t len;
    }xml_object_t;

    typedef struct {
        xml_node_t* child;

    }xml_node_t;


#ifdef __cplusplus
}
#endif

#endif /* XML_H */