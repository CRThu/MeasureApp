#include "../Inc/dynamic_call.h"

delegate_t delegates[DYNAMIC_CALL_FUNC_MAX_CNT];
uint16_t delegates_count = 0;

void delegate_register(delegate delegate, char* name, dtypes_t* args_type, uint8_t args_count)
{
	delegates[delegates_count].name = name;
	delegates[delegates_count].handler = delegate;
	delegates[delegates_count].args_type = args_type;
	delegates[delegates_count].args_count = args_count;
	delegates_count++;
}

delegate_t* find_delegate_by_name(delegate_t* delegates, uint16_t delegates_count, char* name)
{
	// 遍历注册方法
	for (uint16_t i = 0; i < delegates_count; i++)
	{
		// 寻找匹配方法名称
		if (NAME_ISEQUAL(delegates[i].name, name))
		{
			return &delegates[i];
		}
	}

	return NULL;
}


void invoke(dynamic_pool_t* pool, delegate_t* f)
{
	// method invoke
	uint8_t args[10][100];

	for (uint16_t i = 1; i <= f->args_count; i++)
	{
		dynamic_pool_get(pool, i, f->args_type[i - 1], args[i - 1], 100);
	}

	switch (f->args_count)
	{
	case 0:
		((delegate_a0r0)f->handler)();
		break;
	case 1:
		((delegate_a1r0)f->handler)(args[0]);
		break;
	case 2:
		((delegate_a2r0)f->handler)(args[0], args[1]);
		break;
	case 3:
		((delegate_a3r0)f->handler)(args[0], args[1], args[2]);
		break;
	case 4:
		((delegate_a4r0)f->handler)(args[0], args[1], args[2], args[3]);
		break;
	case 5:
		((delegate_a5r0)f->handler)(args[0], args[1], args[2], args[3], args[4]);
		break;
	case 6:
		((delegate_a6r0)f->handler)(args[0], args[1], args[2], args[3], args[4], args[5]);
		break;
	case 7:
		((delegate_a7r0)f->handler)(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
		break;
	case 8:
		((delegate_a8r0)f->handler)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
		break;
	case 9:
		((delegate_a9r0)f->handler)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
		break;
	default:
		// TODO
		break;
	}
}
