#include "xml.h"
#include "stdlib.h"
#include "string.h"

// --------------- MEMORY CREATE AND FREE FUNCTION ---------------

/// <summary>
/// create a node
/// </summary>
/// <param name="node"></param>
/// <returns></returns>
xml_err_t xml_mem_node_create(xml_node_t* node)
{
    node = (xml_node_t*)malloc(sizeof(xml_node_t) * 1);
    if (node == NULL)
    {
        return XML_MALLOC_FAILED;
    }
    return XML_NO_ERR;
}

/// <summary>
/// create an object
/// </summary>
/// <param name="obj"></param>
/// <returns></returns>
xml_err_t xml_mem_object_create(xml_object_t* obj)
{
    obj = (xml_object_t*)malloc(sizeof(xml_object_t) * 1);
    if (obj == NULL)
    {
        return XML_MALLOC_FAILED;
    }
    return XML_NO_ERR;
}


/// <summary>
/// free a node
/// </summary>
/// <param name="node"></param>
void xml_mem_node_free(xml_node_t* node)
{
    free(node);
}

/// <summary>
/// free an object
/// </summary>
/// <param name="node"></param>
void xml_mem_object_free(xml_object_t* obj)
{
    free(obj);
}

void xml_obj_set_ref(xml_object_t* obj, void* ref, size_t len)
{
    obj->buffer = ref;
    obj->len = len;
}

/// <summary>
/// create node
/// </summary>
/// <param name="node"></param>
/// <param name="name"></param>
/// <returns></returns>
xml_err_t xml_create_node(xml_node_t* node, char* name)
{
    xml_err_t err;

    // malloc object
    xml_object_t* name_obj;
    err = xml_mem_object_create(name_obj);
    if (err != XML_NO_ERR)
        return err;
    xml_obj_set_ref(name_obj, name, strlen(name));

    // malloc node
    err = xml_mem_node_create(node);
    if (err != XML_NO_ERR)
        return err;

    // set node
    node->name = name_obj;
    node->content = NULL;
    node->attributes = NULL;
    node->children = NULL;
    node->next = NULL;

    return XML_NO_ERR;
}

// --------------- XML OPERATE FUNCTION ---------------

xml_err_t xml_create_root(xml_node_t* root, char* name)
{
    return xml_create_node(root, name);
}

/// <summary>
/// add child node to node
/// </summary>
/// <param name="node"></param>
/// <param name="name"></param>
/// <returns></returns>
xml_err_t xml_add_child(xml_node_t* node, char* name)
{
    xml_err_t err;

    // TODO find if there is a duplicate element or not

    // create node
    xml_node_t* child;
    err = xml_create_node(child, name);
    if (err != XML_NO_ERR)
        return err;

    // update node
    xml_node_t* last = node->children;
    while (last->next != NULL)
    {
        last = last->next;
    }
    last->next = child;

    return XML_NO_ERR;
}

xml_err_t xml_generate(xml_node_t* root, uint8_t* buffer, size_t bufsize, size_t* len)
{
    *len = 0;
    memcpy(buffer[*len], root->name->buffer, root->name->len);
    *len += root->name->len;
}