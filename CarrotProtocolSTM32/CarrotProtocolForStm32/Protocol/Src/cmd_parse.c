#include "../Inc/cmd_parse.h"

/// <summary>
/// ����ָ��
/// </summary>
/// <param name="pool">dynamic_pool_t�ṹ��</param>
/// <param name="cmd">��������ָ���ַ���</param>
/// <param name="len">ָ���ַ�������</param>
/// <returns>����״̬</returns>
cmd_parse_status_t cmd_parse_one(dynamic_pool_t* pool, char* cmd, uint16_t len)
{
    uint16_t curr_pos = 0;
    uint16_t statement_end_pos = 0;

    while (curr_pos < len)
    {
        if (CMD_PARSE_END(cmd[curr_pos]))
        {
            statement_end_pos = curr_pos;
            parse_params(pool, cmd, len);
        }
        curr_pos++;
    }

    return 0;
}

/// <summary>
/// ָ���������
/// </summary>
/// <param name="pool">dynamic_pool_t�ṹ��</param>
/// <param name="cmd">�������Ĳ����ַ���</param>
/// <param name="len">�����ַ�������</param>
/// <returns>����״̬</returns>
cmd_parse_status_t parse_params(dynamic_pool_t* pool, char* cmd, uint16_t len)
{
    uint8_t args_index = 0;
    uint16_t cursor = 0;
    uint16_t start_pos = 0;
    dynamic_pool_init(pool);
    while (cursor < len)
    {
        if (CMD_PARSE_ELEMENT_DELIMITER(cmd[cursor]))
        {
            char* fromele = &cmd[start_pos];
            uint16_t fromlen = cursor - start_pos;
            uint16_t len = 0;


            dynamic_pool_add(pool, T_STRING, fromele, fromlen);

            args_index++;
            start_pos = cursor + 1;
        }
        cursor++;
    }

    return 0;
}
