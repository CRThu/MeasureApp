#include "../Inc/ascii_protocol.h"

void protocol_parse(uint8_t* buf, uint16_t len, uint16_t* endpos)
{
}

void protocol_write_msg(const char* format, ...)
{
    char fmt_buf[2048];
    uint16_t fmt_buf_len;
    va_list args;

    va_start(args, format);
    fmt_buf_len = vsnprintf(((char*)&fmt_buf), sizeof(fmt_buf), (char*)format, args);
    va_end(args);

    uart_txdma_write((uint8_t*)fmt_buf, fmt_buf_len);
}

void protocol_write_data(uint8_t* data, uint16_t len)
{
    protocol_write_msg("<data>");
    protocol_write_msg("<name>%s</name>", "databuf");
    protocol_write_msg("<desc>%s</desc>", "NULL");
    protocol_write_msg("<format>%s</format>", "binary");
    protocol_write_msg("<len>%lu</len>", len);
    protocol_write_msg("<word><bits>%d</bits></word>", 32);
    protocol_write_msg("<bin>");
    uart_txdma_write(data, len);
    protocol_write_msg("</bin>");
    protocol_write_msg("</data>");
    protocol_write_msg("\r\n");
}