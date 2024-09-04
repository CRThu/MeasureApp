#include "../Inc/dynamic_pool.h"

/// <summary>
/// 初始化动态类型数组
/// </summary>
/// <param name="pool">空动态类型数组结构体</param>
void dynamic_pool_init(dynamic_pool_t* pool)
{
    memset(pool->buf, 0u, DYNAMIC_POOL_MAX_BYTES);
    memset(pool->offset, 0u, DYNAMIC_POOL_MAX_PARAMS);
    memset(pool->len, 0u, DYNAMIC_POOL_MAX_PARAMS);
    pool->count = 0;
}

/// <summary>
/// 添加元素通用函数
/// </summary>
/// <param name="pool">动态类型数组结构体</param>
/// <param name="intype">变量类型（未使用）</param>
/// <param name="data">变量首地址指针</param>
/// <param name="len">变量长度</param>
dynamic_pool_status_t dynamic_pool_add(dynamic_pool_t* pool, dtypes_t intype, void* data, uint16_t len)
{
    // intype = string now
    intype = T_STRING;

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
/// 获取元素
/// </summary>
/// <param name="pool">存储结构</param>
/// <param name="index">元素索引</param>
/// <param name="type">类型</param>
/// <param name="data">数据指针</param>
/// <param name="len">数据长度</param>
void dynamic_pool_get(dynamic_pool_t* pool, uint16_t index, dtypes_t type, void* data, uint16_t len)
{
    if (index < pool->count)
    {
        // TODO
        void* internal_data = &pool->buf[pool->offset[index]];
        uint16_t internal_len = pool->len[index];
        type_conversion(internal_data, (size_t*)data, T_BYTES, type, internal_len, len);
    }
    else
    {
        // NO DATA AT INDEX
    }

}

/// <summary>
/// 打印数据存储池
/// </summary>
/// <param name="pool">存储结构</param>
void dynamic_pool_print(dynamic_pool_t* pool)
{
    for (uint16_t i = 0; i < pool->count; i++)
    {
        void* pd = &(pool->buf[pool->offset[i]]);
        dtypes_t dt = T_STRING;
        uint16_t len = pool->len[i];

        char format[256] = "";
        memcpy(format, pd, len);

        switch (dt)
        {
        case T_STRING:
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



const char* enum_to_string(dtypes_t type)
{
    switch (type)
    {
    case T_NULL: return "T_NULL";
    case T_DEC64: return "T_DEC64";
    case T_HEX64: return "T_HEX64";
    case T_STRING: return "T_STRING";
    case T_ENUM: return "T_ENUM";
    case T_BYTES: return "T_BYTES";
    default: return "UNKNOWN";
    }
}

dtypes_t string_to_enum(const char* str)
{
    if (strcmp(str, "T_NULL") == 0) return T_NULL;
    if (strcmp(str, "T_DEC64") == 0) return T_DEC64;
    if (strcmp(str, "T_HEX64") == 0) return T_HEX64;
    if (strcmp(str, "T_STRING") == 0) return T_STRING;
    if (strcmp(str, "T_ENUM") == 0) return T_ENUM;
    if (strcmp(str, "T_BYTES") == 0) return T_BYTES;
    return -1; // Invalid enum string
}


/// <summary>
/// 类型转换
/// </summary>
/// <param name="input">输入数据指针</param>
/// <param name="output">输出数据指针</param>
/// <param name="intype">输入数据类型</param>
/// <param name="outtype">输出数据类型</param>
/// <param name="input_size">输入数据长度</param>
/// <param name="output_size">输出数据长度</param>
void type_conversion(const void* input, void* output, dtypes_t intype, dtypes_t outtype, size_t input_size, size_t output_size)
{
    char buffer[100];

    // Convert input to string using sprintf
    switch (intype)
    {
    case T_NULL:
        buffer[0] = '\0';
        break;
    case T_DEC64:
        sprintf(buffer, "%"PRId64"", *(int64_t*)input);
        break;
    case T_HEX64:
        sprintf(buffer, "%"PRIX64"", *(int64_t*)input);
        break;
    case T_STRING:
        strncpy(buffer, (char*)input, sizeof(buffer) - 1);
        buffer[sizeof(buffer) - 1] = '\0';
        break;
    case T_ENUM:
        if (sscanf((char*)input, "%d", (int*)output) == 1)
        {
            // Input is a number, convert to enum string
            strncpy(buffer, enum_to_string(*(int*)output), sizeof(buffer) - 1);
        }
        else
        {
            // Input is a string, convert to enum number
            *(int*)output = string_to_enum((char*)input);
            strncpy(buffer, (char*)input, sizeof(buffer) - 1);
        }
        buffer[sizeof(buffer) - 1] = '\0';
        break;
    case T_BYTES:
        memcpy(buffer, input, input_size);
        buffer[input_size] = '\0'; // Ensure null-terminated string for further processing
        break;
    default:
        printf("Unsupported input type\n");
        return;
    }

    // Convert string to output type using sscanf
    switch (outtype)
    {
    case T_NULL:
        // No conversion needed for T_NULL
        break;
    case T_DEC64:
        sscanf(buffer, "%"PRId64"", (int64_t*)output);
        break;
    case T_HEX64:
        sscanf(buffer, "%"PRIX64"", (uint64_t*)output);
        break;
    case T_STRING:
        strncpy((char*)output, buffer, output_size - 1);
        ((char*)output)[output_size - 1] = '\0';
        break;
    case T_ENUM:
        if (sscanf(buffer, "%d", (int*)output) == 1)
        {
            // Output is a number, convert to enum string
            strncpy((char*)output, enum_to_string(*(int*)output), output_size - 1);
        }
        else
        {
            // Output is a string, convert to enum number
            *(int*)output = string_to_enum(buffer);
            strncpy((char*)output, buffer, output_size - 1);
        }
        ((char*)output)[output_size - 1] = '\0';
        break;
    case T_BYTES:
        memcpy(output, buffer, output_size);
        break;
    default:
        printf("Unsupported output type\n");
    }
}