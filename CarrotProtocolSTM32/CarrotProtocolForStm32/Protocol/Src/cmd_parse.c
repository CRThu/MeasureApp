#include "../Inc/cmd_parse.h"


/// <summary>
/// ָ�������ʼ��
/// </summary>
/// <param name="obj">dynamic_pool_t�ṹ��</param>
/// <param name="cmd">��������ָ���ַ���</param>
/// <param name="len">ָ���ַ�������</param>
cmd_parse_status_t cmd_parse_one(dynamic_pool_t* obj, char* cmd, uint16_t len)
{
	uint16_t curr_pos = 0;
	uint16_t statement_end_pos = 0;

	while (curr_pos < len)
	{
		if (CMD_PARSE_END(cmd[curr_pos]))
		{
			statement_end_pos = curr_pos;
			parse_params(obj, cmd, len);
		}
		curr_pos++;
	}

	return 0;
}

/// <summary>
/// ָ�����, ��������Ϊ�ַ���
/// </summary>
/// <param name="buffer">cmd_parse_t�ṹ��</param>
/// <param name="buf">�ַ�������ָ��</param>
/// <param name="len">�ַ������鳤��</param>
/// <returns>�ַ���ʵ�ʳ���</returns>
cmd_parse_status_t parse_params(dynamic_pool_t* obj, char* cmd, uint16_t len)
{
	// types types_len not used

	uint8_t args_index = 0;
	uint16_t cursor = 0;
	uint16_t start_pos = 0;
	dynamic_pool_init(obj);
	while (cursor < len)
	{
		if (CMD_PARSE_ELEMENT_DELIMITER(cmd[cursor]))
		{
			uint8_t* fromele = &cmd[start_pos];
			uint16_t fromlen = cursor - start_pos;
			uint16_t len = 0;


			dynamic_pool_add(obj, T_STRING, fromele, fromlen);

			args_index++;
			start_pos = cursor + 1;
		}
		cursor++;
	}

	return 0;
}
