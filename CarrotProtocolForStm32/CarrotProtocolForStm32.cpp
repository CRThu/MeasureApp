// CarrotProtocolForStm32.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <string>
#include "payload_parse.h"

int main()
{
	payload_parse_t buf;
	const char* str = " A  BB CCC  \r\n";

	payload_parse_init(&buf, (uint8_t*)str, strlen(str));
	char* str1 = (char*)malloc(256);
	char* str2 = (char*)malloc(256);
	char* str3 = (char*)malloc(256);
	char* str4 = (char*)malloc(256);
	memset(str1, 0, 256);
	memset(str2, 0, 256);
	memset(str3, 0, 256);
	memset(str4, 0, 256);
	int len1 = payload_parse_string(&buf, str1, 256);
	int len2 = payload_parse_string(&buf, str2, 256);
	int len3 = payload_parse_string(&buf, str3, 256);
	int len4 = payload_parse_string(&buf, str4, 256);

	std::cout << "CMD: " << str << std::endl;
	std::cout << "PAR1: " << str1 << " ( len= " << len1 << " )" << std::endl;
	std::cout << "PAR2: " << str2 << " ( len= " << len2 << " )" << std::endl;
	std::cout << "PAR3: " << str3 << " ( len= " << len3 << " )" << std::endl;
	std::cout << "PAR4: " << str4 << " ( len= " << len4 << " )" << std::endl;

	free(str1);
	free(str2);
	free(str3);
	free(str4);
}