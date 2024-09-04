/****************************
 * DELAY
 * CARROT HU
 * 2024.08.01
 *****************************/
#include "delay.h"
// static uint8_t fac_us = 0;
// static uint16_t fac_ms = 0;

// TODO
__DELAY_INLINE void delay_init()
{
    // SysTick_Config(SYSTICK_CLKSOURCE_HCLK_DIV8);
    // fac_us = HAL_RCC_GetHCLKFreq() / 8000000;
    // fac_ms = HAL_RCC_GetHCLKFreq() / 8000;
}

// TODO
__DELAY_INLINE void delay_nop(uint32_t cnt)
{
    for(uint32_t i = 0; i < cnt; i++ )
    {
        __NOP();
    }
}

// TODO
__DELAY_INLINE void delay_us(uint32_t utime)
{
    //SysTick_Config(400000000 / 1000000);  //ticks=400-1;1000000;1280000
    SysTick->LOAD  = (SystemCoreClock / 1000000) - 1;                         /* set reload register */
    SysTick->VAL   = 0UL;                                             /* Load the SysTick Counter Value */
    SysTick->CTRL  = SysTick_CTRL_CLKSOURCE_Msk | SysTick_CTRL_ENABLE_Msk;  //enable systick timer
    while(utime)
    {
        if(SysTick->CTRL & SysTick_CTRL_COUNTFLAG_Msk)
            utime--;
    }
    SysTick->CTRL  = SysTick_CTRL_CLKSOURCE_Msk;                   
}

// TODO
__DELAY_INLINE void delay_ms(uint32_t mtime)
{
    HAL_Delay(mtime);
}