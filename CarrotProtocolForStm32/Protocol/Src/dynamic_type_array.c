#pragma once
#include "../Inc/dynamic_type_array.h"

/// <summary>
/// ��ʼ����̬��������
/// </summary>
/// <param name="dyn">�ն�̬��������ṹ��</param>
void dynamic_type_array_init(dynamic_type_array_t* dyn)
{
	memset(dyn->data, 0u, DYNAMIC_TYPE_ARRAY_STORAGE_MAX_BYTES);
	memset(dyn->idx, 0u, DYNAMIC_TYPE_ARRAY_ELEMENTS_MAX_CNT);
	memset(dyn->len, 0u, DYNAMIC_TYPE_ARRAY_ELEMENTS_MAX_CNT);
	memset(dyn->type, 0u, DYNAMIC_TYPE_ARRAY_ELEMENTS_MAX_CNT);
	dyn->count = 0;
}

/// <summary>
/// ���Ԫ��ͨ�ú���
/// </summary>
/// <param name="dyn">��̬��������ṹ��</param>
/// <param name="data">������ַ</param>
/// <param name="len">��������</param>
/// <param name="type">��������</param>
void dynamic_type_array_add(dynamic_type_array_t* dyn, void* data, uint16_t len, dynamic_types_t type)
{
	uint16_t index = 0;
	if (dyn->count != 0)
		index = dyn->idx[dyn->count - 1] + dyn->len[dyn->count - 1];

	dyn->idx[dyn->count] = index;
	dyn->len[dyn->count] = len;
	dyn->type[dyn->count] = type;

	memcpy(dyn->data[index], data, len);

	dyn->count++;
}

void dynamic_type_array_get_type(dynamic_type_array_t* dyn, uint16_t index, void* data, dynamic_types_t* type)
{
	if (index < dyn->count)
	{
		data = dyn->idx[index];
		type = dyn->idx[index];
	}
	else
	{
		data = NULL;
		type = NULLTYPE;
	}
}