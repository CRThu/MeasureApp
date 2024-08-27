#include "../Inc/cmd_parse.h"
#include "../Inc/dynamic_call.h"


/// <summary>
/// 指令解析初始化
/// </summary>
/// <param name="obj">dynamic_pool_t结构体</param>
/// <param name="cmd">待解析的指令字符串</param>
/// <param name="len">指令字符串长度</param>
cmd_parse_status_t cmd_parse_one(dynamic_pool_t* obj, callback_t** methods, uint16_t methods_count, char* cmd, uint16_t len)
{
	uint16_t curr_pos = 0;
	uint16_t statement_end_pos = 0;
	callback_t* method_info = NULL;

	while (curr_pos < len)
	{
		if (CMD_PARSE_END(cmd[curr_pos]))
		{
			statement_end_pos = curr_pos;
			parse_params(obj, method_info, cmd, len);
			invoke_method(obj, method_info);
		}
		else if (CMD_PARSE_ELEMENT_DELIMITER(cmd[curr_pos]))
		{
			method_info = find_method_by_name(methods, methods_count, cmd, curr_pos + 1);
		}
		curr_pos++;
	}
}

/// <summary>
/// 指令解析, 参数类型为字符串
/// </summary>
/// <param name="buffer">cmd_parse_t结构体</param>
/// <param name="buf">字符串数组指针</param>
/// <param name="len">字符串数组长度</param>
/// <returns>字符串实际长度</returns>
cmd_parse_status_t parse_params(dynamic_pool_t* obj, callback_t* method_info, char* cmd, uint16_t len)
{
	uint8_t args_index = 0;
	uint16_t cursor = 0;
	uint16_t start_pos = 0;
	dynamic_pool_init(obj);
	while (cursor < len)
	{
		if (CMD_PARSE_ELEMENT_DELIMITER(cmd[cursor]))
		{
			if (args_index >= method_info->args_count)
			{
				return -1;
			}

			uint8_t* fromele = &cmd[start_pos];
			uint16_t fromlen = cursor - start_pos;
			uint8_t totype = method_info->args[args_index];
			void* data = NULL;
			uint16_t len = 0;

			switch (totype)
			{
			case 'i':
			{
				int32_t num = bytes_to_long(fromele, fromlen, 0, NULL);
				dynamic_pool_add(obj, T_DEC64, &num, 0);
				break;
			}
			default:
				break;
			}

			args_index++;
			start_pos = cursor;
		}
		cursor++;
	}
}
