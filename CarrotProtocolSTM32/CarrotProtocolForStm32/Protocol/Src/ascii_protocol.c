#include "../Inc/ascii_protocol.h"

void protocol_parse(uint8_t* buf, uint16_t len, uint16_t* endpos)
{

}

void protocol_write_msg(char* msg)
{
    uart_txdma_write((uint8_t*)msg, strlen(msg));
}

void protocol_write_data(uint8_t* data, uint16_t len)
{
    protocol_write_msg("<data>");
    protocol_write_msg("<name>databuf</name>");
    protocol_write_msg("<desc></desc>");
    protocol_write_msg("<format>binary</format>");
    protocol_write_msg("<len>0</len>");
    protocol_write_msg("<word><bits>32</bits></word>");
    protocol_write_msg("<bin>");
    uart_txdma_write(data, len);
    protocol_write_msg("</bin>");
    protocol_write_msg("</data>");
}