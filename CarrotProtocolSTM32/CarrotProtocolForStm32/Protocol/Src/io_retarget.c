/****************************
 * IO RETARGET
 * CARROT HU
 * 2024.09.04
 *****************************/
#include <stdio.h>
#include "main.h"
#include "usart.h"

#define IO_RETARGET_UART_INSTANCE huart1

/**
 * @brief  Retargets the C library __io_putchar function to the USART.
 * @param  None
 * @retval None
 */
#ifdef __GNUC__
int __io_putchar(int ch)
#else
int fputc(int ch, FILE *f)
#endif
{
    /* Implementation of __io_putchar */
    /* e.g. write a character to the UART1 and Loop until the end of transmission */
    HAL_UART_Transmit(&IO_RETARGET_UART_INSTANCE, (uint8_t *)&ch, 1, 0xFFFFFFFF);

    return ch;
}

/**
 * @brief  Retargets the C library __io_getchar function to the USART.
 * @param  None
 * @retval character read uart
 */
#ifdef __GNUC__
int __io_getchar(void)
#else
int fgetc(FILE *f)
#endif
{
    /* Implementation of __io_getchar */
    char rxChar;

    // This loops in case of HAL timeout, but if an ok or error occurs, we continue
    while (HAL_UART_Receive(&IO_RETARGET_UART_INSTANCE, (uint8_t *)&rxChar, 1, 0xFFFFFFFF) == HAL_TIMEOUT)
        ;

    return rxChar;
}