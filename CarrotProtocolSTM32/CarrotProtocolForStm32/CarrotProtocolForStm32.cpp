﻿#include "cmd_parse.h"
#include "dynamic_call.h"
#include "DynamicCallTest.h"
#include "xml.h"

#ifdef _DEBUG_VLD_
    #include "vld.h"
#endif

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
    xml_create_root(&root, "root", strlen("root"));
    xml_add_child(root, "head", strlen("head"));
    xml_add_child(root, "body", strlen("body"));
    xml_add_child(root, "body", strlen("body"));
    xml_add_attribute(root, "path", "parent");
    xml_add_attribute(root, "path", "parent.override");
    xml_add_attribute(root->children, "path", "children.first");
    xml_add_attribute(root->children->next, "path", "children.next");
    xml_add_attribute(root->children->next, "name1", "content1");
    xml_add_content(root, "123", strlen("123"));
    xml_add_content(root, "456", strlen("456"));
    xml_add_content(root, "789", strlen("789"));

    xml_node_t* node1 = xml_get_node(root, "");
    xml_node_t* node2 = xml_get_node(root, "/");
    xml_node_t* node3 = xml_get_node(root, "/root");
    xml_node_t* node4 = xml_get_node(root, "/root/");
    xml_node_t* node5 = xml_get_node(root, "/root/body");
    xml_node_t* node6 = xml_get_node(root, "/root/body/");
    xml_node_t* node_wrong1 = xml_get_node(root, "/123");
    xml_node_t* node_new1 = xml_get_node(root, "/root/new");
    //xml_node_t* node_new2 = xml_get_node(root, "/root/new/");

    const char* CDATA_PAYLOAD = "<TEST PAYLOAD>";
    xml_node_t* bdata_test_node = xml_get_node(root, "/root/bdata");
    xml_node_t* cdata_test_node = xml_get_node(root, "/root/cdata");
    xml_add_bdata(bdata_test_node, (uint8_t*)CDATA_PAYLOAD, strlen(CDATA_PAYLOAD));
    xml_add_cdata(cdata_test_node, CDATA_PAYLOAD, strlen(CDATA_PAYLOAD));

    xml_generate(root, buf, sizeof(buf), &consumed, XML_FORMAT_INDENT_SPACE_4);
    printf("BUFFER:\r\n");
    for (int i = 0; i < consumed; i++)
        printf("%c", buf[i]);
    printf("\r\n");
    xml_free_node(root);
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
