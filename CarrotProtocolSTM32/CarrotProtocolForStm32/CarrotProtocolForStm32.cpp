#include "cmd_parse.h"
#include "dynamic_call.h"
#include "DynamicCallTest.h"
#include "xml.h"

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

void xml_test()
{
    xml_node_t* root = NULL;
    uint8_t buf[4096];
    size_t consumed = 0;
    xml_create_root(&root, "root");
    xml_add_child(root, "head");
    xml_add_child(root, "body");
    xml_add_child(root, "body");
    xml_add_attribute(root, "path", "parent");
    xml_add_attribute(root, "path", "parent.override");
    xml_add_attribute(root->children, "path", "children.first");
    xml_add_attribute(root->children->next, "path", "children.next");
    xml_add_attribute(root->children->next, "name1", "content1");
    xml_add_content(root, "12345");
    xml_add_content(root, "67890");
    xml_generate(root, buf, sizeof(buf), &consumed);
    printf("BUFFER:");
    for (int i = 0; i < consumed; i++)
        printf("%c", buf[i]);
    xml_free_node(root);
}

void create(xml_node_t** node)
{
    *node = (xml_node_t*)malloc(sizeof(xml_node_t) * 1);
}


int main()
{
    //dynamic_call_register();

    //dynamic_pool_t pool;
    //parse_test(&pool);
    //invoke_test(&pool);
    //dyncall_test(&pool);

    xml_test();
}
