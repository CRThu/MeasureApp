/****************************
 * UART COMM
 * CARROT HU
 * 2024.08.12
 *****************************/
#pragma once
#ifndef _UART_COMM_H_
#define _UART_COMM_H_

#ifdef __cplusplus
extern "C"
{
#endif
#define UART_COMM_VERSION "2.0.0"

#include <stdint.h>
#include <string.h>
#include "main.h"
#include "usart.h"

#define UART_HANDLE         huart1
#define RXDMA_BUFSIZE       2048
#define TXDMA_BUFSIZE       2048

#define RXDMA_POS_END       (RXDMA_BUFSIZE - 1)

extern uint8_t rxdma_buf[RXDMA_BUFSIZE];
extern uint8_t txdma_buf[TXDMA_BUFSIZE];

#define RXDMA_BUF_GET_AVAILABLE_LEN()   (RXDMA_BUFSIZE - __HAL_DMA_GET_COUNTER(huart1.hdmarx))
#define RXCMD_READ_AVAILABLE()          (rxcmd_src_len != 0)
#define CIRCUIR_ARRAY_INDEX(idx, size)  ((idx) < (size) ? (idx) : ((idx) - (size)))
#define RXDMA_AT(idx)                   (rxdma_buf[CIRCUIR_ARRAY_INDEX((idx), (RXDMA_BUFSIZE))])
#define MEMCPY_FROM_RINGBUF(dst, src, src_size, src_head, len)                          \
    do {                                                                                \
        size_t first_chunk_size = (src_size) - (src_head);                              \
        if((len) <= (first_chunk_size)) {                                               \
            memcpy((dst), &(src)[(src_head)], (len));                                   \
        } else {                                                                        \
            memcpy((dst), &(src)[(src_head)], (first_chunk_size));                      \
            memcpy((dst) + (first_chunk_size), &(src)[0], (len) - (first_chunk_size));  \
        }                                                                               \
    } while (0)                                                                         \

void uart_rxdma_init();
void uart_rxdma_update_pos_wr();
void uart_rxdma_parse();
uint16_t uart_rxdma_read(uint8_t* rxcmd_buf, uint16_t len);
void uart_txdma_write(uint8_t* txcmd_buf, uint16_t len);



#ifdef __cplusplus
}
#endif

#endif // _UART_COMM_H_
