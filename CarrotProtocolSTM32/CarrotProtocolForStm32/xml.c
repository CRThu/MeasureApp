#include "xml.h"
#include "stdlib.h"
#include "string.h"

// --------------- MEMORY CREATE AND FREE FUNCTION ---------------

/// <summary>
/// create a node
/// </summary>
/// <param name="node"></param>
/// <returns></returns>
xml_err_t xml_mem_node_create(xml_node_t** node)
{
    *node = (xml_node_t*)malloc(sizeof(xml_node_t) * 1);
    if (*node == NULL)
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
xml_err_t xml_mem_object_create(xml_object_t** obj)
{
    *obj = (xml_object_t*)malloc(sizeof(xml_object_t) * 1);
    if (*obj == NULL)
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

void xml_obj_set_ref(xml_object_t* obj, const void* ref, size_t len)
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
xml_err_t xml_create_node(xml_node_t** node, const char* name)
{
    xml_err_t err;

    // malloc object
    xml_object_t* name_obj = NULL;
    err = xml_mem_object_create(&name_obj);
    if (err != XML_NO_ERR)
        return err;
    xml_obj_set_ref(name_obj, name, strlen(name));

    // malloc node
    err = xml_mem_node_create(node);
    if (err != XML_NO_ERR)
        return err;

    // set node
    (*node)->name = name_obj;
    (*node)->content = NULL;
    (*node)->attributes = NULL;
    (*node)->children = NULL;
    (*node)->next = NULL;

    return XML_NO_ERR;
}

// --------------- XML OPERATE FUNCTION ---------------

xml_err_t xml_create_root(xml_node_t** root, char* name)
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
    xml_node_t* child = NULL;
    err = xml_create_node(&child, name);
    if (err != XML_NO_ERR)
        return err;

    // update node
    if (node->children == NULL)
    {
        node->children = child;
    }
    else
    {
        xml_node_t* last = node->children;
        while (last->next != NULL)
        {
            last = last->next;
        }
        last->next = child;
    }

    return XML_NO_ERR;
}

xml_err_t xml_generate_ltag(xml_node_t* node, uint8_t* buffer, size_t bufsize, size_t* consumed)
{
    if (node == NULL)
        return XML_NO_ERR;

    // <
    buffer[*consumed] = '<';
    (*consumed)++;

    // tagname
    memcpy(&buffer[*consumed], node->name->buffer, node->name->len);
    *consumed += node->name->len;

    xml_attribute_t* attr = node->attributes;
    if (attr != NULL)
    {
        // ATTR1="VAL1" ATTR2="VAL2"
        while (attr != NULL)
        {
            buffer[*consumed] = ' ';
            (*consumed)++;


            // ATTR1
            memcpy(&buffer[*consumed], node->attributes->name->buffer, node->attributes->name->len);
            *consumed += node->name->len;

            // =
            buffer[*consumed] = '=';
            (*consumed)++;

            // "
            buffer[*consumed] = '\"';
            (*consumed)++;

            // VAL1
            memcpy(&buffer[*consumed], node->attributes->content, node->attributes->content->len);
            *consumed += node->name->len;

            // "
            buffer[*consumed] = '\"';
            (*consumed)++;

            // goto next attr
            attr = attr->next;
        }
    }

    // >
    buffer[*consumed] = '>';
    (*consumed)++;

    // CONTENT
    xml_object_t* content = node->content;
    if (content != NULL)
    {
        memcpy(&buffer[*consumed], node->content->buffer, node->content->len);
        *consumed += node->content->len;
    }

    return XML_NO_ERR;
}

xml_err_t xml_generate_rtag(xml_node_t* node, uint8_t* buffer, size_t bufsize, size_t* consumed)
{
    if (node == NULL)
        return XML_NO_ERR;

    // <
    buffer[*consumed] = '<';
    (*consumed)++;

    // /
    buffer[*consumed] = '/';
    (*consumed)++;

    // tagname
    memcpy(&buffer[*consumed], node->name->buffer, node->name->len);
    *consumed += node->name->len;

    // >
    buffer[*consumed] = '>';
    (*consumed)++;

}

xml_err_t xml_generate(xml_node_t* root, uint8_t* buffer, size_t bufsize, size_t* consumed)
{
    xml_generate_ltag(root, buffer, bufsize, consumed);
    if (root->children != NULL)
        xml_generate(root->children, buffer, bufsize, consumed);
    xml_generate_rtag(root, buffer, bufsize, consumed);
    if (root->next != NULL)
        xml_generate(root->next, buffer, bufsize, consumed);

    return XML_NO_ERR;
}