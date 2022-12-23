#pragma once
#include <inttypes.h>
#include <string.h>

#ifndef DYNAMIC_TYPE_ARRAY_H
#define DYNAMIC_TYPE_ARRAY_H

#define DYNAMIC_TYPE_ARRAY_STORAGE_MAX_BYTES 256
#define DYNAMIC_TYPE_ARRAY_ELEMENTS_MAX_CNT 32

#ifdef __cplusplus
extern "C"
{
#endif
	typedef enum dynamic_types_t
	{
		NULLTYPE,
		UINT32,
		INT32,
		FLOAT64,
		STRING,
	}dynamic_types_t;


	/// <summary>
	/// ��̬��������ṹ��
	/// </summary>
	typedef struct {
		/// <summary>
		/// ���ݴ洢
		/// </summary>
		uint8_t data[DYNAMIC_TYPE_ARRAY_STORAGE_MAX_BYTES];

		/// <summary>
		/// ÿ��Ԫ�ؿ�ʼ��ַ
		/// </summary>
		uint16_t idx[DYNAMIC_TYPE_ARRAY_ELEMENTS_MAX_CNT];
		/// <summary>
		/// ÿ��Ԫ���ֽڳ���
		/// </summary>
		uint16_t len[DYNAMIC_TYPE_ARRAY_ELEMENTS_MAX_CNT];
		/// <summary>
		/// ÿ��Ԫ������
		/// </summary>
		dynamic_types_t type[DYNAMIC_TYPE_ARRAY_ELEMENTS_MAX_CNT];

		/// <summary>
		/// Ŀǰ�洢Ԫ������
		/// </summary>
		uint16_t count;
	}dynamic_type_array_t;

	void dynamic_type_array_init(dynamic_type_array_t* dyn);
	void dynamic_type_array_add(dynamic_type_array_t* dyn, void* data, uint16_t len, dynamic_types_t type);


#ifdef __cplusplus
}
#endif

#endif /* DYNAMIC_TYPE_ARRAY_H */