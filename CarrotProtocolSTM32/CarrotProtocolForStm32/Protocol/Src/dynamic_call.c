#include "../Inc/dynamic_call.h"

callback_t callbacks[DYNAMIC_CALL_FUNC_MAX_CNT];
uint16_t callbacks_count = 0;


void dynamic_register(callback fn, char* name, char* args)
{
	callbacks[callbacks_count].name = name;
	callbacks[callbacks_count].func = fn;
	callbacks[callbacks_count].args = args;
	callbacks[callbacks_count].args_count = strlen(args);
	callbacks_count++;
}

static callback_t* find_method_by_name(callback_t** methods, uint16_t methods_count, char* fn_name, uint16_t fn_name_len)
{
	// 遍历注册方法
	for (uint16_t i = 0; i < methods_count; i++)
	{
		// 寻找匹配方法名称
		if (mem_equal(methods[i]->name, fn_name, fn_name_len))
		{
			return methods[i];
		}
	}
}

/*
void invoke_method(dynamic_pool_t* obj, callback_t* method)
{
	char fn_name[256] = { 0 };
	uint16_t len = payload_parse_string(args, fn_name, 255);

	// 遍历注册方法
	for (int i = 0; i < callbacks_count; i++)
	{
		// 寻找匹配方法名称
		if (FN_NAME_ISEQUAL(callbacks[i].name, fn_name))
		{
			dynamic_pool_t dyn;
			dynamic_pool_init(&dyn);

			// 参数解析
			uint16_t fn_arg_cnt = FN_ARGS_CNT(callbacks[i].args);
			for (int arg_type_idx = 0; arg_type_idx < fn_arg_cnt; arg_type_idx++)
			{
				// 对于不同方法参数选择不同的解析函数, 存储至动态类型数组
				switch (callbacks[i].args[arg_type_idx])
				{
				case 'i':
				{
					int32_t num = payload_parse_int32(args);
					dynamic_pool_add_value(&dyn, &num, sizeof(num), INT32TYPE);
					break;
				}
				case 'f':
				{
					double num = payload_parse_double(args);
					dynamic_pool_add_value(&dyn, &num, sizeof(num), FLOAT64TYPE);
					break;
				}
				case 's':
				default:
				{
					char str[256] = { 0 };
					uint16_t len = payload_parse_string(args, str, 255);
					dynamic_pool_add_value(&dyn, str, len, STRINGTYPE);
					break;
				}
				}
			}

			// method invoke
			void* args[10];
			dynamic_pool_get(&dyn, 0, (void**)&args[0]);
			dynamic_pool_get(&dyn, 1, (void**)&args[1]);
			dynamic_pool_get(&dyn, 2, (void**)&args[2]);
			dynamic_pool_get(&dyn, 3, (void**)&args[3]);
			dynamic_pool_get(&dyn, 4, (void**)&args[4]);
			dynamic_pool_get(&dyn, 5, (void**)&args[5]);
			dynamic_pool_get(&dyn, 6, (void**)&args[6]);
			dynamic_pool_get(&dyn, 7, (void**)&args[7]);
			dynamic_pool_get(&dyn, 8, (void**)&args[8]);
			switch (fn_arg_cnt)
			{
			case 0:
				((callback_a0r0)callbacks[i].func)();
				break;
			case 1:
				((callback_a1r0)callbacks[i].func)(args[0]);
				break;
			case 2:
				((callback_a2r0)callbacks[i].func)(args[0], args[1]);
				break;
			case 3:
				((callback_a3r0)callbacks[i].func)(args[0], args[1], args[2]);
				break;
			case 4:
				((callback_a4r0)callbacks[i].func)(args[0], args[1], args[2], args[3]);
				break;
			case 5:
				((callback_a5r0)callbacks[i].func)(args[0], args[1], args[2], args[3], args[4]);
				break;
			case 6:
				((callback_a6r0)callbacks[i].func)(args[0], args[1], args[2], args[3], args[4], args[5]);
				break;
			case 7:
				((callback_a7r0)callbacks[i].func)(args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
				break;
			case 8:
				((callback_a8r0)callbacks[i].func)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
				break;
			case 9:
				((callback_a9r0)callbacks[i].func)(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
				break;
			default:
				// TODO
				break;
			}

			break;
		}
	}
}
*/