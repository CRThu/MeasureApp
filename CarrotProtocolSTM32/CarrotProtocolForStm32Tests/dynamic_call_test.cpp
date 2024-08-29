#include "CppUnitTest.h"
#include <Protocol/Inc/dynamic_call.h>
#include <DynamicCallTest.h>
#include <Protocol/Inc/cmd_parse.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace CarrotProtocolForStm32Tests
{
	TEST_CLASS(dynamic_call_test)
	{
	public:

		TEST_METHOD(TEST_NUM)
		{
			dynamic_call_register();
			Assert::IsTrue(callbacks_count == 6);

			cmd_parse_t buf;
			const char* cmd0 = "print";
			payload_parse_init(&buf, (uint8_t*)cmd0, strlen(cmd0));
			dynamic_call(&buf);
			Assert::AreEqual("print() Called.", dynamic_call_test_buf);

			const char* cmd1 = "printi 12345";
			payload_parse_init(&buf, (uint8_t*)cmd1, strlen(cmd1));
			dynamic_call(&buf);
			Assert::AreEqual("printi(12345) Called.", dynamic_call_test_buf);

			const char* cmd2 = "prints helloworld";
			payload_parse_init(&buf, (uint8_t*)cmd2, strlen(cmd2));
			dynamic_call(&buf);
			Assert::AreEqual("prints(helloworld) Called.", dynamic_call_test_buf);

			const char* cmd3 = "addi 123 -456";
			payload_parse_init(&buf, (uint8_t*)cmd3, strlen(cmd3));
			dynamic_call(&buf);
			Assert::AreEqual("addi(123, -456) Called.", dynamic_call_test_buf);

			const char* cmd4 = "addf 123 -456";
			payload_parse_init(&buf, (uint8_t*)cmd4, strlen(cmd4));
			dynamic_call(&buf);
			Assert::AreEqual("addf(123.000000, -456.000000) Called.", dynamic_call_test_buf);
		}
	};
}
