#pragma once
#define _CRT_SECURE_NO_WARNINGS
#include "../Inc/dynamic_pool.h"

/// <summary>
/// ��ʼ����̬��������
/// </summary>
/// <param name="dyn">�ն�̬��������ṹ��</param>
void dynamic_pool_init(dynamic_pool_t* pool)
{
	memset(pool->buf, 0u, DYNAMIC_POOL_MAX_BYTES);
	memset(pool->offset, 0u, DYNAMIC_POOL_MAX_PARAMS);
	memset(pool->len, 0u, DYNAMIC_POOL_MAX_PARAMS);
	pool->count = 0;
}

/// <summary>
/// ���Ԫ��ͨ�ú���
/// </summary>
/// <param name="pool">��̬��������ṹ��</param>
/// <param name="type">��������</param>
/// <param name="data">�����׵�ַָ��</param>
/// <param name="len">��������</param>
dynamic_pool_status_t dynamic_pool_add(dynamic_pool_t* pool, dtypes_t intype, void* data, uint16_t len)
{
	if (pool->count >= DYNAMIC_POOL_MAX_PARAMS)
	{
		return -1;
	}

	uint16_t start_offset = strlen(pool->buf);

	if (!DTYPES_IS_REF(intype))
	{
		len = DTYPES_GET_LEN(intype);
		if (len == 0)
			return -2;
	}

	if (start_offset + len > DYNAMIC_POOL_MAX_BYTES - 1)
	{
		return -3;
	}

	if (DTYPES_IS_REF(intype))
	{
		memcpy(&pool->buf[start_offset], (char*)data, len);
	}
	else
	{
		memcpy(&pool->buf[start_offset], data, len);
	}

	pool->offset[pool->count] = start_offset;
	pool->len[pool->count] = len;
	pool->count++;

	return 0;
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
		void* pd;
		uint16_t len;

		dynamic_pool_get(dyn, i, &dt, &pd, &len);

		char format[256] = "";
		switch (dt)
		{
		case T_STRING:
			strcpy(format, pd);
			break;
		case T_DEC64:
			sprintf(format, "%d", *((int32_t*)pd));
			break;
		case T_HEX64:
			sprintf(format, "0x%X", (int)*((uint64_t*)pd));
			break;
		}

		printf("INDEX:%d, TYPE:%02X, ADDR:%08X, LEN:%02X, DATA:%s\r\n", i, dt, (uint32_t)pd, len, format);
	}
}
