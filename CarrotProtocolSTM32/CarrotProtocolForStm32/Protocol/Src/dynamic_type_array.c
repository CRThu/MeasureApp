#pragma once
#include "../Inc/dynamic_type_array.h"

/// <summary>
/// 初始化动态类型数组
/// </summary>
/// <param name="dyn">空动态类型数组结构体</param>
void dynamic_type_array_init(dynamic_type_array_t* dyn)
{
	memset(dyn->data, 0u, DYNAMIC_POOL_MAX_BYTES);
	memset(dyn->idx, 0u, DYNAMIC_POOL_MAX_PARAMS);
	memset(dyn->len, 0u, DYNAMIC_POOL_MAX_PARAMS);
	memset(dyn->type, 0u, DYNAMIC_POOL_MAX_PARAMS);
	dyn->count = 0;
}

/// <summary>
/// 添加元素通用函数
/// </summary>
/// <param name="dyn">动态类型数组结构体</param>
/// <param name="data">变量地址</param>
/// <param name="len">变量长度</param>
/// <param name="type">变量类型</param>
void dynamic_type_array_add(dynamic_type_array_t* dyn, void* data, uint16_t len, dynamic_types_t type)
{
	uint16_t index = 0;
	if (dyn->count != 0)
		index = dyn->idx[dyn->count - 1] + dyn->len[dyn->count - 1];

	dyn->idx[dyn->count] = index;
	dyn->len[dyn->count] = len;
	dyn->type[dyn->count] = type;

	if (DYNAMIC_TYPES_IS_PTR(type))
		memcpy(&dyn->data[index], (char*)data, len);
	else
		memcpy(&dyn->data[index], data, len);

	dyn->count++;
}

/// <summary>
/// 获取元素
/// </summary>
/// <param name="dyn"></param>
/// <param name="index">元素下标</param>
/// <param name="data"></param>
void dynamic_type_array_get(dynamic_type_array_t* dyn, uint16_t index, void** data)
{
	if (index < dyn->count)
	{
		*data = &dyn->data[dyn->idx[index]];
	}
	else
	{
		*data = NULL;
	}
}