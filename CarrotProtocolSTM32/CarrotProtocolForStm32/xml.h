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

#define XML_BOOL_TRUE       (1)
#define XML_BOOL_FALSE      (0)

#define XML_NO_ERR          (0)
#define XML_MALLOC_FAILED   (-1)

    typedef int8_t xml_err_t;
    typedef int8_t xml_bool_t;
    typedef struct _xml_object_t xml_object_t;
    typedef struct _xml_attribute_t xml_attribute_t;
    typedef struct _xml_node_t  xml_node_t;

    typedef struct _xml_object_t {
        const uint8_t* buffer;
        size_t len;
    };

    struct _xml_attribute_t {
        xml_object_t* name;
        xml_object_t* content;

        // struct has next elements if parent is an array
        xml_attribute_t* next;
    };

    struct _xml_node_t {
        xml_object_t* name;
        xml_object_t* content;
        xml_attribute_t* attributes;
        xml_node_t* children;

        // struct has next elements if parent is an array
        xml_node_t* next;
    };

    xml_err_t xml_create_root(xml_node_t** root, const char* name);
    xml_err_t xml_add_child(xml_node_t* node, const char* name);
    xml_err_t xml_add_attribute(xml_node_t* node, const char* name, const char* content);
    xml_err_t xml_generate(xml_node_t* root, uint8_t* buffer, size_t bufsize, size_t* consumed);

#ifdef __cplusplus
}
#endif

#endif /* XML_H */