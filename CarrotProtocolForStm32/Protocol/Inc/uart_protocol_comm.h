#include "main.h"
#include "carrot_protocol.h"

#ifndef UART_PROTOCOL_COMM_H
#define UART_PROTOCOL_COMM_H

#ifdef __cplusplus
extern "C"
{
#endif

#define UART_PROTOCOL_INSTANCE huart1
#define UART_PROTOCOL_PKT_TX_LEN 256
#define UART_PROTOCOL_PKT_RX_LEN 256

    void uart_protocol_rx_dma_start(void);
    void uart_protocol_rx_dma_stop(void);
    uint8_t uart_protocol_available(void);
    void uart_protocol_receive(void *pData);
    void uart_protocol_send(void *pData);

#ifdef __cplusplus
}
#endif

#endif /* UART_PROTOCOL_COMM_H */
