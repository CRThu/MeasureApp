/****************************
 * DELAY
 * CARROT HU
 * 2024.08.01
 *****************************/
#ifndef _DELAY_H_
#define _DELAY_H_

#ifdef __cplusplus
extern "C" {
#endif

#define DELAY_VERSION "1.0.0"

#include <stdint.h>
#include "main.h"

#ifndef   __INLINE
  #define __INLINE                               __inline
#endif
#ifndef   __FORCEINLINE
  #define __FORCEINLINE                         __attribute__((always_inline)) __inline
#endif

#define __DELAY_INLINE                          __FORCEINLINE


void delay_init();
void delay_nop(uint32_t cnt);
void delay_us(uint32_t utime);
void delay_ms(uint32_t mtime);


#ifdef __cplusplus
}
#endif

#endif // _DELAY_H_
