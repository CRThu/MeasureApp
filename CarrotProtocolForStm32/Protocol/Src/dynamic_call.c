#include "../Inc/dynamic_call.h"

callback_t callbacks[DYNAMIC_CALL_FUNC_MAX_CNT];
uint16_t callbacks_count = 0;


void dynamic_register(callback fn, char* name, char* args)
{
	callbacks[callbacks_count].name = name;
	callbacks[callbacks_count].func = fn;
	callbacks[callbacks_count].args = args;
	callbacks_count++;
}

void dynamic_call(payload_parse_t* args)
{
	char fn_name[256] = { 0 };
	uint16_t len = payload_parse_string(args, fn_name, 255);

	// 遍历注册方法
	for (int i = 0; i < callbacks_count; i++)
	{
		// 寻找匹配方法名称
		if (FN_NAME_ISEQUAL(callbacks[i].name, fn_name))
		{
			dynamic_type_array_t dyn;
			dynamic_type_array_init(&dyn);

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
					dynamic_type_array_add(&dyn, &num, sizeof(num), INT32TYPE);
					break;
				}
				case 'f':
				{
					double num = payload_parse_double(args);
					dynamic_type_array_add(&dyn, &num, sizeof(num), FLOAT64TYPE);
					break;
				}
				case 's':
				default:
				{
					char str[256] = { 0 };
					uint16_t len = payload_parse_string(args, str, 255);
					dynamic_type_array_add(&dyn, str, len, STRINGTYPE);
					break;
				}
				}
			}

			// method invoke
			// TESTING
			switch (fn_arg_cnt)
			{
			case 0:
				((callback_a0r0)callbacks[i].func)();
				break;
			case 1:
			{
				void* arg1 = NULL;
				dynamic_type_array_get(&dyn, 0, (void**)&arg1);
				((callback_a1r0)callbacks[i].func)(arg1);
				break;
			}
			case 2:
			{
				void* arg1 = NULL;
				void* arg2 = NULL;
				dynamic_type_array_get(&dyn, 0, (void**)&arg1);
				dynamic_type_array_get(&dyn, 1, (void**)&arg2);
				((callback_a2r0)callbacks[i].func)(arg1, arg2);
				break;
			}
			default:
				// TODO
				break;
			}

			break;
		}
	}
}