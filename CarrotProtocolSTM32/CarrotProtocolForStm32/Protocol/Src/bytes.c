#include "../Inc/bytes.h"

static uint8_t mem_equal(const void* mem1, const void* mem2, size_t len)
{
	const uint8_t* pmem1 = (const uint8_t*)mem1;
	const uint8_t* pmem2 = (const uint8_t*)mem2;
	for (size_t i = 0; i < len; i++)
	{
		if (pmem1[i] != pmem2[i])
		{
			return 0;
		}
	}
	return 1;
}

long bytes_to_long(const uint8_t* bytes, size_t len, int base, size_t* bytes_processed)
{
	long result = 0;
	int negative = 0;
	size_t i = 0;

	// ������ܵķ���
	if (len > 0 && bytes[0] == '-') {
		negative = 1;
		i++;
	}

	// �Զ����ʮ������ǰ׺
	if (base == 0 && len - i >= 2 && bytes[i] == '0' && (bytes[i + 1] == 'x' || bytes[i + 1] == 'X')) {
		base = 16;
		i += 2;
	}

	// ���������Ϊ0��Ĭ��Ϊ10����
	if (base == 0) {
		base = 10;
	}

	// �������Ƿ���Ч
	if (base < 2 || base > 36) {
		errno = EINVAL;
		*bytes_processed = i;
		return 0;
	}

	// ת������
	for (; i < len; i++) {
		int digit;
		if (bytes[i] >= '0' && bytes[i] <= '9') {
			digit = bytes[i] - '0';
		}
		else if (bytes[i] >= 'A' && bytes[i] <= 'Z') {
			digit = bytes[i] - 'A' + 10;
		}
		else if (bytes[i] >= 'a' && bytes[i] <= 'z') {
			digit = bytes[i] - 'a' + 10;
		}
		else {
			// �����Ƿ��ַ���ֹͣת��
			break;
		}

		if (digit >= base) {
			// ���ֳ���������Χ
			break;
		}

		// ������
		if (result > (LONG_MAX - digit) / base) {
			errno = ERANGE;
			*bytes_processed = i;
			return negative ? LONG_MIN : LONG_MAX;
		}

		result = result * base + digit;
	}

	*bytes_processed = i;
	return negative ? -result : result;
}


double bytes_to_double(const uint8_t* bytes, size_t len, size_t* bytes_processed)
{
	double result = 0.0;
	double fraction = 0.0;
	int exponent = 0;
	int negative = 0;
	int seen_dot = 0;
	int seen_e = 0;
	size_t i = 0;
	double frac_multiplier = 0.1;

	// ������ܵķ���
	if (len > 0 && bytes[0] == '-') {
		negative = 1;
		i++;
	}
	else if (len > 0 && bytes[0] == '+') {
		i++;
	}

	// ת��������С������
	for (; i < len; i++) {
		if (bytes[i] >= '0' && bytes[i] <= '9') {
			if (!seen_dot) {
				result = result * 10.0 + (bytes[i] - '0');
			}
			else if (!seen_e) {
				fraction += (bytes[i] - '0') * frac_multiplier;
				frac_multiplier *= 0.1;
			}
			else {
				exponent = exponent * 10 + (bytes[i] - '0');
			}
		}
		else if (bytes[i] == '.' && !seen_dot && !seen_e) {
			seen_dot = 1;
		}
		else if ((bytes[i] == 'e' || bytes[i] == 'E') && !seen_e) {
			seen_e = 1;
			// ����ָ���ķ���
			if (i + 1 < len && bytes[i + 1] == '-') {
				i++;
				exponent = -1;
			}
			else if (i + 1 < len && bytes[i + 1] == '+') {
				i++;
			}
			else {
				exponent = 0;
			}
		}
		else {
			// �����Ƿ��ַ���ֹͣת��
			break;
		}
	}

	*bytes_processed = i;
	result += fraction;

	// Ӧ��ָ��
	if (seen_e) {
		result *= pow(10.0, exponent);
	}

	return negative ? -result : result;
}
