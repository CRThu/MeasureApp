#include "uart_protocol_comm.h"
#include "usart.h"
#include <string.h>

/// UART DMA CONFIG
/// UART TX, DMA, Normal, Byte Increment Address
/// UART RX, DMA, Circular, Byte Increment Address

uint8_t uart_protocol_rx_dma_buf[UART_PROTOCOL_PKT_RX_LEN] = {0};
carrot_data_protocol_256 *rxp = (carrot_data_protocol_256 *)uart_protocol_rx_dma_buf;
volatile uint8_t uart_protocol_rx_data_is_read = 1;

uint8_t uart_protocol_tx_dma_buf[UART_PROTOCOL_PKT_TX_LEN] = {0};
volatile uint8_t uart_protocol_tx_dma_busy = 0;

void uart_protocol_rx_dma_start(void)
{
	HAL_UART_Receive_DMA(&UART_PROTOCOL_INSTANCE, uart_protocol_rx_dma_buf, UART_PROTOCOL_PKT_RX_LEN);
}

void uart_protocol_rx_dma_stop(void)
{
	HAL_UART_DMAStop(&UART_PROTOCOL_INSTANCE);
}

uint8_t uart_protocol_available(void)
{
	return !uart_protocol_rx_data_is_read;
}

void uart_protocol_receive(void *pData)
{
	if (!uart_protocol_rx_data_is_read)
	{
		memcpy(pData, &uart_protocol_rx_dma_buf, UART_PROTOCOL_PKT_RX_LEN);
		uart_protocol_rx_data_is_read = 1;
	}
}

void uart_protocol_send(void *pData)
{
	while (uart_protocol_tx_dma_busy)
		;
	memcpy(&uart_protocol_tx_dma_buf, pData, UART_PROTOCOL_PKT_TX_LEN);
	HAL_UART_Transmit_DMA(&UART_PROTOCOL_INSTANCE, uart_protocol_tx_dma_buf, UART_PROTOCOL_PKT_TX_LEN);
    uart_protocol_tx_dma_busy=1;
}

/**
 * @brief  Tx Transfer completed callbacks.
 * @param  huart  Pointer to a UART_HandleTypeDef structure that contains
 *                the configuration information for the specified UART module.
 * @retval None
 */
void HAL_UART_TxCpltCallback(UART_HandleTypeDef *huart)
{
	// usb_printf("[INFO]: HAL_UART_TxCpltCallback() Called\r\n");
	// UART_TX_CALLBACK_FLAG = 1;
	uart_protocol_tx_dma_busy = 0;
}

/**
 * @brief  Rx Transfer completed callbacks.
 * @param  huart  Pointer to a UART_HandleTypeDef structure that contains
 *                the configuration information for the specified UART module.
 * @retval None
 */
void HAL_UART_RxCpltCallback(UART_HandleTypeDef *huart)
{
	// usb_printf("[INFO]: HAL_UART_RxCpltCallback() Called\r\n");
	// UART_RX_CALLBACK_FLAG = 1;
	uart_protocol_rx_data_is_read = 0;
}

/**
 * @brief  UART error callbacks.
 * @param  huart  Pointer to a UART_HandleTypeDef structure that contains
 *                the configuration information for the specified UART module.
 * @retval None
 */
void HAL_UART_ErrorCallback(UART_HandleTypeDef *huart)
{
	// printf("[ERROR]: HAL_UART_ErrorCallback()\r\n");
}