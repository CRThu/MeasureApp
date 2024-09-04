#include "uart_comm.h"

uint8_t rxdma_buf[RXDMA_BUFSIZE];
uint8_t txdma_buf[TXDMA_BUFSIZE];
uint16_t rxdma_pos_wr, rxdma_pos_rd;
uint16_t rxcmd_src_head, rxcmd_src_len;

void uart_rxdma_init()
{
    HAL_UART_Receive_DMA(&UART_HANDLE, rxdma_buf, RXDMA_BUFSIZE);
    rxdma_pos_wr = 0;
    rxdma_pos_rd = 0;
    rxcmd_src_head = 0;
    rxcmd_src_len = 0;
}

void uart_rxdma_update_pos_wr()
{
    // NDTR     876543218
    // wr       012345670
    rxdma_pos_wr = RXDMA_BUF_GET_AVAILABLE_LEN();
}

void uart_rxdma_parse()
{
    uart_rxdma_update_pos_wr();
    // start from read cursor
    uint16_t pos_start = rxdma_pos_rd;
    // end to write cursor (maybe across bound)
    uint16_t pos_end = rxdma_pos_rd <= rxdma_pos_wr ? rxdma_pos_wr : RXDMA_BUFSIZE + rxdma_pos_wr;
    for(uint16_t i = 0; i < pos_end - pos_start; i++)
    {
        // goto frame end
        if(RXDMA_AT(pos_start + i) == '\n')
        {
            rxcmd_src_head = pos_start;
            rxcmd_src_len = i + 1;
            rxdma_pos_rd = CIRCUIR_ARRAY_INDEX(pos_start + i + 1, RXDMA_BUFSIZE);
            break;
        }
    }
}

uint16_t uart_rxdma_read(uint8_t* rxcmd_buf, uint16_t len)
{
    uint16_t rxlen = 0;
    if(!RXCMD_READ_AVAILABLE())
    {
        uart_rxdma_parse();
    }
    if(RXCMD_READ_AVAILABLE() && (rxcmd_src_len <= len))
    {
        MEMCPY_FROM_RINGBUF(rxcmd_buf, rxdma_buf, RXDMA_BUFSIZE, rxcmd_src_head, rxcmd_src_len);
        rxlen = rxcmd_src_len;
        rxcmd_src_head = 0;
        rxcmd_src_len = 0;
    }
    return rxlen;
}

void uart_txdma_write(uint8_t* txcmd_buf, uint16_t len)
{
    while(huart1.gState != HAL_UART_STATE_READY)
    {
        // WAITING FOR AVAILABLE
        __NOP();
    }
    memcpy(txdma_buf, txcmd_buf, len);
    while (HAL_UART_Transmit_DMA(&huart1, txdma_buf, len) != HAL_OK)
    {
        // ERROR
        __NOP();
    }
}