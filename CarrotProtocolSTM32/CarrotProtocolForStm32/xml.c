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

    (*node)->name = NULL;
    (*node)->content = NULL;
    (*node)->attributes = NULL;
    (*node)->children = NULL;
    (*node)->next = NULL;

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

    (*obj)->value = NULL;
    (*obj)->len = 0;
    (*obj)->next = NULL;

    return XML_NO_ERR;
}

/// <summary>
/// create an attribute
/// </summary>
/// <param name="attr"></param>
/// <returns></returns>
xml_err_t xml_mem_attribute_create(xml_attribute_t** attr)
{
    *attr = (xml_attribute_t*)malloc(sizeof(xml_attribute_t) * 1);
    if (*attr == NULL)
    {
        return XML_MALLOC_FAILED;
    }

    (*attr)->name = NULL;
    (*attr)->value = NULL;
    (*attr)->next = NULL;

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

/// <summary>
/// free an attribute
/// </summary>
/// <param name="attr"></param>
void xml_mem_attribute_free(xml_attribute_t* attr)
{
    free(attr);
}

void xml_obj_set_ref(xml_object_t* obj, const void* ref, size_t len)
{
    obj->value = ref;
    obj->len = len;
}

/// <summary>
/// create node
/// </summary>
/// <param name="node"></param>
/// <param name="name"></param>
/// <returns></returns>
xml_err_t xml_create_node(xml_node_t** node, const char* name, size_t len)
{
    xml_err_t err;

    // malloc object
    xml_object_t* name_obj = NULL;
    err = xml_mem_object_create(&name_obj);
    if (err != XML_NO_ERR)
        return err;
    xml_obj_set_ref(name_obj, name, len);

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

xml_err_t xml_create_attribute(xml_attribute_t** attr, const char* name, const char* content)
{
    xml_err_t err;

    // malloc object
    xml_object_t* name_obj = NULL;
    err = xml_mem_object_create(&name_obj);
    if (err != XML_NO_ERR)
        return err;
    xml_obj_set_ref(name_obj, name, strlen(name));

    xml_object_t* value_obj = NULL;
    err = xml_mem_object_create(&value_obj);
    if (err != XML_NO_ERR)
        return err;
    xml_obj_set_ref(value_obj, content, strlen(content));

    // malloc attribute
    err = xml_mem_attribute_create(attr);
    if (err != XML_NO_ERR)
        return err;

    // set node
    (*attr)->name = name_obj;
    (*attr)->value = value_obj;
    (*attr)->next = NULL;

    return XML_NO_ERR;
}

xml_node_t* xml_is_child_exist(xml_node_t* root, char* name, size_t len)
{
    xml_node_t* node = root->children;
    while (node != NULL)
    {
        if (strncmp(node->name->value, name, len) == 0
            && node->name->len == len)
            return node;
        node = node->next;
    }
    return NULL;
}

xml_attribute_t* xml_is_attribute_exist(xml_node_t* root, char* name, size_t len)
{
    xml_attribute_t* attr = root->attributes;
    while (attr != NULL)
    {
        if (strncmp(attr->name->value, name, len) == 0
            && attr->name->len == len)
            return attr;
        attr = attr->next;
    }
    return NULL;
}


xml_node_t* xml_get_node(xml_node_t* root, const char* path)
{
    uint8_t search_root = 1;
    xml_node_t* curr = root;
    const char* start = path;

    while (*start)
    {
        // find next '/' in the path
        const char* end = strchr(start, '/');
        if (end == NULL)
            end = start + strlen(start);
        size_t len = (end - start);
        if (len == 0)
        {
            // input="/"
            start = end + 1;
            continue;
        }

        // search for existing children nodes
        xml_node_t* result;

        if (search_root)
        {
            result = (strncmp(curr->name->value, start, len) == 0 && curr->name->len == len) ? root : NULL;
            if (result != NULL)
            {
                search_root = 0;
            }
            else
            {
                // there is no specifed root
                return NULL;
            }
        }
        else
        {
            curr = xml_add_child(curr, start, len);
        }


        start = (end + 1);
    }

    return curr;
}


// --------------- XML OPERATE FUNCTION ---------------

xml_err_t xml_create_root(xml_node_t** root, char* name, size_t len)
{
    return xml_create_node(root, name, len);
}

/// <summary>
/// add child node to node
/// </summary>
/// <param name="node"></param>
/// <param name="name"></param>
/// <returns></returns>
xml_node_t* xml_add_child(xml_node_t* node, const char* name, size_t len)
{
    xml_err_t err;

    // to find if there is a duplicate element or not
    xml_node_t* child = xml_is_child_exist(node, name, len);
    if (child == NULL)
    {
        // create node
        //xml_node_t* child = NULL;
        err = xml_create_node(&child, name, len);
        if (err != XML_NO_ERR)
            return NULL;

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
    }
    else
    {

    }

    return child;
}

/// <summary>
/// 
/// </summary>
/// <param name="node"></param>
/// <param name="name"></param>
/// <param name="value"></param>
/// <returns></returns>
xml_err_t xml_add_attribute(xml_node_t* node, char* name, const char* value)
{
    xml_err_t err;

    // to find if there is a duplicate element or not
    xml_attribute_t* attr = xml_is_attribute_exist(node, name, strlen(name));
    if (attr == NULL)
    {
        // create attribute
        //xml_attribute_t* attr = NULL;
        err = xml_create_attribute(&attr, name, value);
        if (err != XML_NO_ERR)
            return err;

        // update node
        if (node->attributes == NULL)
        {
            node->attributes = attr;
        }
        else
        {
            xml_attribute_t* last = node->attributes;
            while (last->next != NULL)
            {
                last = last->next;
            }
            last->next = attr;
        }
    }
    else
    {
        xml_obj_set_ref(attr->value, value, strlen(value));
    }

    return XML_NO_ERR;
}

xml_err_t xml_add_content(xml_node_t* node, const char* content)
{
    xml_err_t err;

    xml_object_t* content_obj = NULL;
    err = xml_mem_object_create(&content_obj);
    if (err != XML_NO_ERR)
        return err;
    xml_obj_set_ref(content_obj, content, strlen(content));

    if (node->content == NULL)
    {
        node->content = content_obj;
    }
    else
    {
        node->content->next = content_obj;
    }
}

xml_err_t xml_generate_ltag(xml_node_t* node, uint8_t* buffer, size_t bufsize, size_t* consumed)
{
    if (node == NULL)
        return XML_NO_ERR;

    // <
    buffer[*consumed] = '<';
    (*consumed)++;

    // tagname
    memcpy(&buffer[*consumed], node->name->value, node->name->len);
    *consumed += node->name->len;

    xml_attribute_t* attr = node->attributes;
    if (attr != NULL)
    {
        // ATTR1="VAL1" ATTR2="VAL2"
        while (attr != NULL)
        {
            buffer[*consumed] = ' ';
            (*consumed)++;


            // ATTRx
            memcpy(&buffer[*consumed], attr->name->value, attr->name->len);
            *consumed += attr->name->len;

            // =
            buffer[*consumed] = '=';
            (*consumed)++;

            // "
            buffer[*consumed] = '\"';
            (*consumed)++;

            // CONTENTx
            memcpy(&buffer[*consumed], attr->value->value, attr->value->len);
            *consumed += attr->value->len;

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
    while (content != NULL)
    {
        memcpy(&buffer[*consumed], content->value, content->len);
        *consumed += content->len;

        content = content->next;
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
    memcpy(&buffer[*consumed], node->name->value, node->name->len);
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


void xml_free_object(xml_object_t* obj)
{
    if (obj != NULL)
    {
        if (obj->next != NULL)
        {
            xml_free_object(obj->next);
            obj->next = NULL;
        }

        obj->len = 0;
        xml_mem_object_free(obj);
        obj = NULL;
    }
}

void xml_free_attribute(xml_attribute_t* attr)
{
    if (attr != NULL)
    {
        if (attr->next != NULL)
        {
            xml_free_attribute(attr->next);
            attr->next = NULL;
        }

        xml_mem_object_free(attr->name);
        attr->name = NULL;
        xml_mem_object_free(attr->value);
        attr->value = NULL;
        xml_mem_attribute_free(attr);
        attr = NULL;
    }
}

void xml_free_node(xml_node_t* node)
{
    if (node != NULL)
    {
        if (node->children != NULL)
        {
            xml_free_node(node->children);
            node->children = NULL;
        }

        if (node->next != NULL)
        {
            xml_free_node(node->next);
            node->next = NULL;
        }

        xml_free_object(node->name);
        node->name = NULL;
        xml_free_object(node->content);
        node->content = NULL;
        xml_free_attribute(node->attributes);
        node->attributes = NULL;
    }
}

