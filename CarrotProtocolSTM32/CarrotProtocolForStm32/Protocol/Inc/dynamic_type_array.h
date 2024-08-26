#pragma once
#include <inttypes.h>
#include <string.h>

#ifndef DYNAMIC_TYPE_ARRAY_H
#define DYNAMIC_TYPE_ARRAY_H

#define DYNAMIC_POOL_MAX_BYTES 1024
#define DYNAMIC_POOL_MAX_PARAMS 256

// DYNAMIC TYPES IS VALUE OR POINTER
#define DTYPES_STORE_VAL            (0x00 << 7)
#define DTYPES_STORE_PTR            (0x01 << 7)
#define DTYPES_STORE_MASK           (0x01 << 7)

// DYNAMIC TYPES LENGTH
#define DTYPES_STORE_LEN_1B         (0x00 << 5)
#define DTYPES_STORE_LEN_4B         (0x01 << 5)
#define DTYPES_STORE_LEN_RESERVED   (0x02 << 5)
#define DTYPES_STORE_LEN_PTR        (0x03 << 5)
#define DTYPES_STORE_MASK           (0x03 << 5)

// DYNAMIC CALLS PARAMS DATATYPES
#define DTYPES_STORE_NULL           (0x00 << 0)
#define DTYPES_STORE_DEC64          (0x01 << 0)
#define DTYPES_STORE_HEX64          (0x02 << 0)
#define DTYPES_STORE_STRING         (0x03 << 0)
#define DTYPES_STORE_ENUM           (0x04 << 0)
#define DTYPES_STORE_BYTES          (0x05 << 0)
#define DTYPES_STORE_JSON           (0x06 << 0)
#define DTYPES_STORE_MASK           (0x1F << 0)


#ifdef __cplusplus
extern "C"
{
#endif
    typedef enum dynamic_types_t
    {
        T_NULL = ( DTYPES_STORE_VAL | DTYPES_STORE_LEN_1B | DTYPES_STORE_NULL),
        T_DEC64 = (DTYPES_STORE_VAL | DTYPES_STORE_LEN_4B | DTYPES_STORE_DEC64),
        T_HEX64 = (DTYPES_STORE_VAL | DTYPES_STORE_LEN_4B | DTYPES_STORE_HEX64),
        T_STRING = (DTYPES_STORE_PTR | DTYPES_STORE_LEN_PTR | DTYPES_STORE_STRING),
        T_ENUM = (DTYPES_STORE_VAL | DTYPES_STORE_LEN_4B | DTYPES_STORE_ENUM),
        T_BYTES = (DTYPES_STORE_PTR | DTYPES_STORE_LEN_PTR | DTYPES_STORE_BYTES),
        T_JSON = (DTYPES_STORE_PTR | DTYPES_STORE_LEN_PTR | DTYPES_STORE_JSON),
    }dynamic_types_t;


    /// <summary>
    /// 动态类型数组结构体
    /// </summary>
    typedef struct {
        /// <summary>
        /// 数据存储
        /// </summary>
        uint8_t data[DYNAMIC_POOL_MAX_BYTES];

        /// <summary>
        /// 每个元素开始地址
        /// </summary>
        uint16_t idx[DYNAMIC_POOL_MAX_PARAMS];
        /// <summary>
        /// 每个元素字节长度
        /// </summary>
        uint16_t len[DYNAMIC_POOL_MAX_PARAMS];
        /// <summary>
        /// 每个元素类型
        /// </summary>
        dynamic_types_t type[DYNAMIC_POOL_MAX_PARAMS];

        /// <summary>
        /// 目前存储元素数量
        /// </summary>
        uint16_t count;
    }dynamic_type_array_t;

    void dynamic_type_array_init(dynamic_type_array_t* dyn);
    void dynamic_type_array_add(dynamic_type_array_t* dyn, void* data, uint16_t len, dynamic_types_t type);
    void dynamic_type_array_get(dynamic_type_array_t* dyn, uint16_t index, void** data);

#ifdef __cplusplus
}
#endif

#endif /* DYNAMIC_TYPE_ARRAY_H */