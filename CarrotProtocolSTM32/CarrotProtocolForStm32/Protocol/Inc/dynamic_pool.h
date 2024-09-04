#pragma once
#include <inttypes.h>
#include <stdio.h>
#include <string.h>

#ifndef DYNAMIC_POOL_H
#define DYNAMIC_POOL_H

#define DYNAMIC_POOL_VERSION    "1.0.1"

#ifndef _CRT_SECURE_NO_WARNINGS
#define _CRT_SECURE_NO_WARNINGS
#endif // !_CRT_SECURE_NO_WARNINGS


#define DYNAMIC_POOL_MAX_BYTES 1024
#define DYNAMIC_POOL_MAX_PARAMS 256

// DYNAMIC TYPES IS VALUE OR POINTER
#define DTYPES_STORE_VAL            (0x00 << 7)
#define DTYPES_STORE_REF            (0x01 << 7)
#define DTYPES_STORE_REF_MASK       (0x01 << 7)
#define DTYPES_IS_REF(x)            (((x) & DTYPES_STORE_REF_MASK) == DTYPES_STORE_REF)

// DYNAMIC TYPES LENGTH
#define DTYPES_STORE_LEN_1B         (0x00 << 5)
#define DTYPES_STORE_LEN_8B         (0x01 << 5)
#define DTYPES_STORE_LEN_RESERVED   (0x02 << 5)
#define DTYPES_STORE_LEN_PTR        (0x03 << 5)
#define DTYPES_STORE_LEN_MASK       (0x03 << 5)
#define DTYPES_GET_LEN(x)           ((((x) & DTYPES_STORE_LEN_MASK) == DTYPES_STORE_LEN_1B) ? 1 \
                                        : ((((x) & DTYPES_STORE_LEN_MASK) == DTYPES_STORE_LEN_8B) ? 8 \
                                            : 0))
// DYNAMIC CALLS PARAMS DATATYPES
#define DTYPES_STORE_NULL           (0x00 << 0)
#define DTYPES_STORE_DEC64          (0x01 << 0)
#define DTYPES_STORE_HEX64          (0x02 << 0)
#define DTYPES_STORE_STRING         (0x03 << 0)
#define DTYPES_STORE_ENUM           (0x04 << 0)
#define DTYPES_STORE_BYTES          (0x05 << 0)
#define DTYPES_STORE_JSON           (0x06 << 0)
#define DTYPES_STORE_DTYPE_MASK     (0x1F << 0)

#define T_NULL                      (DTYPES_STORE_VAL | DTYPES_STORE_LEN_1B | DTYPES_STORE_NULL)
#define T_DEC64                     (DTYPES_STORE_VAL | DTYPES_STORE_LEN_8B | DTYPES_STORE_DEC64)
#define T_HEX64                     (DTYPES_STORE_VAL | DTYPES_STORE_LEN_8B | DTYPES_STORE_HEX64)
#define T_STRING                    (DTYPES_STORE_REF | DTYPES_STORE_LEN_PTR | DTYPES_STORE_STRING)
#define T_ENUM                      (DTYPES_STORE_VAL | DTYPES_STORE_LEN_8B | DTYPES_STORE_ENUM)
#define T_BYTES                     (DTYPES_STORE_REF | DTYPES_STORE_LEN_PTR | DTYPES_STORE_BYTES)
#define T_JSON                      (DTYPES_STORE_REF | DTYPES_STORE_LEN_PTR | DTYPES_STORE_JSON)

#ifdef __cplusplus
extern "C"
{
#endif
	typedef uint8_t dtypes_t;
	typedef int8_t dynamic_pool_status_t;

	/// <summary>
	/// 动态类型数组结构体
	/// </summary>
	typedef struct {
		/// <summary>
		/// 字符串原格式数据存储
		/// </summary>
		char buf[DYNAMIC_POOL_MAX_BYTES];

		/// <summary>
		/// 每个元素开始地址
		/// </summary>
		uint16_t offset[DYNAMIC_POOL_MAX_PARAMS];

		/// <summary>
		/// 每个元素字节长度
		/// </summary>
		uint16_t len[DYNAMIC_POOL_MAX_PARAMS];

		/// <summary>
		/// 目前存储元素数量
		/// </summary>
		uint16_t count;
	}dynamic_pool_t;

	void dynamic_pool_init(dynamic_pool_t* dyn);
	dynamic_pool_status_t dynamic_pool_add(dynamic_pool_t* dyn, dtypes_t type, void* data, uint16_t len);
	void dynamic_pool_get(dynamic_pool_t* dyn, uint16_t index, dtypes_t type, void* data, uint16_t len);

	void dynamic_pool_print(dynamic_pool_t* dyn);

	const char* enum_to_string(dtypes_t type);
	dtypes_t string_to_enum(const char* str);
	void type_conversion(const void* input, void* output, dtypes_t intype, dtypes_t outtype, size_t input_size, size_t output_size);

#ifdef __cplusplus
}
#endif

#endif /* DYNAMIC_POOL_H */