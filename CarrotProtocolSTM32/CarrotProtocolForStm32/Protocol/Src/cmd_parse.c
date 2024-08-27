#include "../Inc/cmd_parse.h"
#include "../Inc/dynamic_call.h"


/// <summary>
/// ָ�������ʼ��
/// </summary>
/// <param name="obj">dynamic_pool_t�ṹ��</param>
/// <param name="cmd">��������ָ���ַ���</param>
/// <param name="len">ָ���ַ�������</param>
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
/// ָ�����, ��������Ϊ�ַ���
/// </summary>
/// <param name="buffer">cmd_parse_t�ṹ��</param>
/// <param name="buf">�ַ�������ָ��</param>
/// <param name="len">�ַ������鳤��</param>
/// <returns>�ַ���ʵ�ʳ���</returns>
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
