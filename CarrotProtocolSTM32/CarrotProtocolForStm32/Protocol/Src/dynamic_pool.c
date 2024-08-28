#pragma once
#include "../Inc/dynamic_pool.h"

/// <summary>
/// ��ʼ����̬��������
/// </summary>
/// <param name="dyn">�ն�̬��������ṹ��</param>
void dynamic_pool_init(dynamic_pool_t* dyn)
{
	memset(dyn->data, 0u, DYNAMIC_POOL_MAX_BYTES);
	memset(dyn->idx, 0u, DYNAMIC_POOL_MAX_PARAMS);
	memset(dyn->len, 0u, DYNAMIC_POOL_MAX_PARAMS);
	memset(dyn->type, 0u, DYNAMIC_POOL_MAX_PARAMS);
	dyn->count = 0;
}

/// <summary>
/// ���Ԫ��ͨ�ú���
/// </summary>
/// <param name="dyn">��̬��������ṹ��</param>
/// <param name="type">��������</param>
/// <param name="data">������ַ</param>
/// <param name="len">��������</param>
dynamic_pool_status_t dynamic_pool_add(dynamic_pool_t* dyn, dtypes_t type, void* data, uint16_t len)
{
	if (dyn->count >= DYNAMIC_POOL_MAX_PARAMS)
		return -1;

	uint16_t index = 0;
	if (dyn->count != 0)
		index = dyn->idx[dyn->count - 1] + dyn->len[dyn->count - 1];

	if (!DTYPES_IS_REF(type))
	{
		len = DTYPES_GET_LEN(type);
		if (len == 0)
			return -2;
	}

	if (index + len > DYNAMIC_POOL_MAX_BYTES - 1)
		return -3;

	if (DTYPES_IS_REF(type))
	{
		memcpy(&dyn->data[index], (char*)data, len);
	}
	else
	{
		memcpy(&dyn->data[index], data, len);
	}

	dyn->idx[dyn->count] = index;
	dyn->len[dyn->count] = len;
	dyn->type[dyn->count] = type;
	dyn->count++;
}

/// <summary>
/// ��ȡԪ��
/// </summary>
/// <param name="dyn"></param>
/// <param name="index">Ԫ���±�</param>
/// <param name="data"></param>
void dynamic_pool_get(dynamic_pool_t* dyn, uint16_t index, dtypes_t* type, void** data, uint16_t* len)
{
	if (index < dyn->count)
	{
		*type = dyn->type[index];
		*data = &dyn->data[dyn->idx[index]];
		*len = dyn->len[index];
	}
	else
	{
		*type = T_NULL;
		*data = NULL;
		*len = 0;
	}

}


void dynamic_pool_print(dynamic_pool_t* dyn)
{
	for (uint16_t i = 0; i < dyn->count; i++)
	{
		dtypes_t dt;
		uint8_t* pd;
		uint16_t len;

		dynamic_pool_get(dyn, i, &dt, &pd, &len);
	}
}
