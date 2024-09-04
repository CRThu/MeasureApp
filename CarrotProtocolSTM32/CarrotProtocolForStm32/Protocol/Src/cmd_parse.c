#include "../Inc/cmd_parse.h"

/// <summary>
/// 解析指令
/// </summary>
/// <param name="pool">dynamic_pool_t结构体</param>
/// <param name="cmd">待解析的指令字符串</param>
/// <param name="len">指令字符串长度</param>
/// <returns>解析状态</returns>
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
/// 指令参数解析
/// </summary>
/// <param name="pool">dynamic_pool_t结构体</param>
/// <param name="cmd">待解析的参数字符串</param>
/// <param name="len">参数字符串长度</param>
/// <returns>解析状态</returns>
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
