#include "cmd_parse.h"
#include "dynamic_call.h"
#include "DynamicCallTest.h"

void parse_test(dynamic_pool_t* pool)
{
    dynamic_pool_init(pool);

    uint8_t types[] = { T_STRING, T_DEC64, T_HEX64 };
    uint8_t types_len = sizeof(types);

    printf("TYPE:\nT_STRING=%02X\nT_DEC64=%02X\nT_HEX64=%02X\n", types[0], types[1], types[2]);

    char c[256] = "";
    char* p = c;
    strcpy(p, "func(32,10)\n");

    cmd_parse_one(pool, p, strlen(p));

    printf("CMD: %s\n", p);

    dynamic_pool_print(pool);

    printf("\n");
}

void invoke_test(dynamic_pool_t* pool)
{
    invoke(pool, &(delegates[0]));
    invoke(pool, &(delegates[1]));
    invoke(pool, &(delegates[2]));
    invoke(pool, &(delegates[3]));
}

void dyncall_test(dynamic_pool_t* pool)
{
    char s[256];

    dynamic_pool_init(pool);
    printf("\nWrite a command to execute:");
    scanf("%s", &s);
    cmd_parse_one(pool, s, 256);

    char funcname[256];
    dynamic_pool_get(pool, 0, T_STRING, funcname, 256);
    delegate_t* sel = find_delegate_by_name(delegates, delegates_count, funcname);

    printf("found function: %s\n", sel->name);

    invoke(pool, sel);
}

int main()
{
    dynamic_call_register();

    dynamic_pool_t pool;
    //parse_test(&pool);
    //invoke_test(&pool);
    dyncall_test(&pool);
}
