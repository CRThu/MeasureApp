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

#define XML_VERSION		    "1.0.0"

#define XML_NO_ERR          (0)
#define XML_MALLOC_FAILED   (-1)

    typedef int8_t xml_err_t;

    typedef struct {
        const uint8_t* buffer;
        size_t len;
    }xml_object_t;

    typedef struct {
        xml_object_t* name;
        xml_object_t* content;

        // struct has next elements if parent is an array
        xml_attribute_t* next;
    }xml_attribute_t;

    typedef struct {
        xml_object_t* name;
        xml_object_t* content;
        xml_attribute_t* attributes;
        xml_node_t* children;

        // struct has next elements if parent is an array
        xml_node_t* next;
    }xml_node_t;


#ifdef __cplusplus
}
#endif

#endif /* XML_H */