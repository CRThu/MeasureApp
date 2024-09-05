/****************************
 * IO RETARGET
 * CARROT HU
 * 2024.09.05
 *****************************/
#include <stdio.h>
#include "main.h"
#include "usart.h"

#define IO_RETARGET_UART_INSTANCE huart1


 /*
  * Arm Compiler 4/5 or Arm Compiler (armclang)
  */
#if   defined ( __CC_ARM ) || defined (__ARMCC_VERSION)

#define PUTCHAR_PROTOTYPE   int fputc(int ch, FILE *f)
#define GETCHAR_PROTOTYPE   int fgetc(FILE *f)

  /*
   * GNU Compiler
   */
#elif defined ( __GNUC__ )

#define PUTCHAR_PROTOTYPE   int __io_putchar(int ch)
#define GETCHAR_PROTOTYPE   int __io_getchar(void)

#else
#error Unknown compiler.
#endif


  /**
   * @brief  Retargets the C library __io_putchar function to the USART.
   * @param  None
   * @retval None
   */
PUTCHAR_PROTOTYPE
{
    while (huart1.gState != HAL_UART_STATE_READY)
    {
        // WAITING FOR AVAILABLE
        __NOP();
    }
/* Implementation of __io_putchar */
/* e.g. write a character to the UART1 and Loop until the end of transmission */
HAL_UART_Transmit(&IO_RETARGET_UART_INSTANCE, (uint8_t*)&ch, 1, 0xFFFFFFFF);

return ch;
}

/**
 * @brief  Retargets the C library __io_getchar function to the USART.
 * @param  None
 * @retval character read uart
 */
    GETCHAR_PROTOTYPE
{
    while (huart1.gState != HAL_UART_STATE_READY)
    {
        // WAITING FOR AVAILABLE
        __NOP();
    }
/* Implementation of __io_getchar */
char rxChar;

// This loops in case of HAL timeout, but if an ok or error occurs, we continue
while (HAL_UART_Receive(&IO_RETARGET_UART_INSTANCE, (uint8_t*)&rxChar, 1, 0xFFFFFFFF) == HAL_TIMEOUT)
    ;

return rxChar;
}