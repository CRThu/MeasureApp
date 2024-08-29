#include "CppUnitTest.h"
#include <Protocol/Inc/cmd_parse.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace CarrotProtocolForStm32Tests
{
	TEST_CLASS(payload_parse_test)
	{
	public:

		TEST_METHOD(TestPayloadParseString)
		{
			const char* str = " A  BB CCC  123 456";
			const char* expstr[5] = { "A","BB","CCC","123","456" };
			const int explen[5] = { 1,2,3,3,3 };

			payload_parse_t buf;
			payload_parse_init(&buf, (uint8_t*)str, strlen(str));

			char* str1 = (char*)malloc(256);
			int len1;
			memset(str1, 0, 256);

			for (int i = 0; i < 5; i++)
			{
				len1 = payload_parse_string(&buf, str1, 256);
				Assert::AreEqual(expstr[i], str1);
				Assert::AreEqual(explen[i], len1);
			}

			free(str1);
		}


		TEST_METHOD(TestPayloadUInt32)
		{
			const char* str = " 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010";
			const uint32_t exp[11] = { 1u, UINT32_MAX, 1u, UINT32_MAX, UINT32_MAX, UINT32_MAX, 123u, 0u, 15u, 8u, 0u };
			payload_parse_t buf;
			payload_parse_init(&buf, (uint8_t*)str, strlen(str));

			char* str1 = (char*)malloc(256);
			uint32_t num;
			memset(str1, 0, 256);

			for (int i = 0; i < 11; i++)
			{
				num = payload_parse_uint32(&buf);
				Assert::AreEqual(exp[i], num);
			}

			free(str1);
		}

		TEST_METHOD(TestPayloadInt32)
		{
			const char* str = " 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010";
			const int32_t exp[11] = { 1, -1, 1, -1, INT32_MAX, INT32_MIN, 123, 0, 15, 8, 0 };
			payload_parse_t buf;
			payload_parse_init(&buf, (uint8_t*)str, strlen(str));

			char* str1 = (char*)malloc(256);
			int32_t num;
			memset(str1, 0, 256);

			for (int i = 0; i < 11; i++)
			{
				num = payload_parse_int32(&buf);
				Assert::AreEqual(exp[i], num);
			}

			free(str1);
		}

		TEST_METHOD(TestPayloadDouble)
		{
			const char* str = " 1 -1  1.234 -1.234 999999999999999999 -999999999999999999 123ABC ABC 0xF 010";
			const double exp[11] = { 1, -1, 1.234, -1.234, 999999999999999999, -999999999999999999, 123, 0, 15, 10, 0 };
			payload_parse_t buf;
			payload_parse_init(&buf, (uint8_t*)str, strlen(str));

			char* str1 = (char*)malloc(256);
			double num;
			memset(str1, 0, 256);

			for (int i = 0; i < 11; i++)
			{
				num = payload_parse_double(&buf);
				Assert::AreEqual(exp[i], num);
			}

			free(str1);
		}
	};
}
