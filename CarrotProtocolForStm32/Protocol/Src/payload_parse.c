#include "payload_parse.h"

/// <summary>
/// ָ�������ʼ��
/// </summary>
/// <param name="buffer">payload_parse_t�ṹ��</param>
/// <param name="payload">��������ָ���ַ���</param>
/// <param name="len">ָ���ַ�������</param>
void payload_parse_init(payload_parse_t* buffer, uint8_t* payload, uint16_t len)
{
	buffer->buffer = payload;
	buffer->length = len;
	buffer->cursor = 0;
}

/// <summary>
/// ָ�����, ��������Ϊ�ַ���
/// </summary>
/// <param name="buffer">payload_parse_t�ṹ��</param>
/// <param name="buf">�ַ�������ָ��</param>
/// <param name="len">�ַ������鳤��</param>
/// <returns>�ַ���ʵ�ʳ���</returns>
uint16_t payload_parse_string(payload_parse_t* buffer, char* buf, uint16_t len)
{
	uint16_t start_index = buffer->cursor;
	uint16_t end_index = buffer->cursor;
	uint8_t fsm = 0;

	// ��֤����ָ�����ַ������鳤����
	while (buffer->cursor < buffer->length && fsm != 2)
	{
		uint8_t c = buffer->buffer[buffer->cursor];
		switch (fsm)
		{
		case 0:
			// ɾ��ָ�������ո�
			if (PAYLOAD_CHECK_SPACE(c))
			{
				buffer->cursor++;
			}
			else
			{
				start_index = buffer->cursor;
				fsm++;
			}
			break;
		case 1:
			// ��¼����λ��
			if (PAYLOAD_CHECK_SPACE(c))
			{
				end_index = buffer->cursor;
				fsm++;
			}
			else
			{
				buffer->cursor++;
			}
			break;
		}
	}

	// û�н������ͼ�⵽�ַ���β��end�α�ָ�����
	if (fsm == 1)
	{
		end_index = buffer->cursor;
	}

	uint16_t actual_len = (end_index - start_index) > len ? len : (end_index - start_index);
	memcpy(buf, &buffer->buffer[start_index], actual_len);
	return actual_len;
}

uint16_t payload_parse_uint16(payload_parse_t* buffer)
{

}