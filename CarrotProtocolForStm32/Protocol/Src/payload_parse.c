#include "payload_parse.h"

/// <summary>
/// 指令解析初始化
/// </summary>
/// <param name="buffer">payload_parse_t结构体</param>
/// <param name="payload">待解析的指令字符串</param>
/// <param name="len">指令字符串长度</param>
void payload_parse_init(payload_parse_t* buffer, uint8_t* payload, uint16_t len)
{
	buffer->buffer = payload;
	buffer->length = len;
	buffer->cursor = 0;
}

/// <summary>
/// 指令解析, 参数类型为字符串
/// </summary>
/// <param name="buffer">payload_parse_t结构体</param>
/// <param name="buf">字符串数组指针</param>
/// <param name="len">字符串数组长度</param>
/// <returns>字符串实际长度</returns>
uint16_t payload_parse_string(payload_parse_t* buffer, char* buf, uint16_t len)
{
	uint16_t start_index = buffer->cursor;
	uint16_t end_index = buffer->cursor;
	uint8_t fsm = 0;

	// 保证数组指针在字符串数组长度内
	while (buffer->cursor < buffer->length && fsm != 2)
	{
		uint8_t c = buffer->buffer[buffer->cursor];
		switch (fsm)
		{
		case 0:
			// 删除指令参数间空格
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
			// 记录参数位置
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

	// 没有结束符就检测到字符串尾则end游标指向最后
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