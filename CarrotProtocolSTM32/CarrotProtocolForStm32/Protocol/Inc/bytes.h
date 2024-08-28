#pragma once
#include <inttypes.h>
#include <stdio.h>
#include <stdint.h>
#include <stddef.h>
#include <limits.h>
#include <errno.h>

#ifndef BYTES_H
#define BYTES_H

#ifdef __cplusplus
extern "C"
{
#endif

	uint8_t mem_equal(const void* mem1, const void* mem2, size_t len);
	long bytes_to_long(const uint8_t* bytes, size_t len, int base, size_t* bytes_processed);
	double bytes_to_double(const uint8_t* bytes, size_t len, size_t* bytes_processed);



#ifdef __cplusplus
}
#endif

#endif /* BYTES_H */