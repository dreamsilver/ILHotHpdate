using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主题： Unity 帮助脚本
///  Description: 
///  功能： 提供程序用户一些常用的功能方法实现，方便开发。
/// </summary>
public class UnityHelper
{
    /*
     * 1:查找子节点对象
     * 2:获取子节点对象脚本
     * 3:给子节点添加脚本
     * 4:给子节点添加父物体
     */

    /// <summary>
    /// 查找子节点对象
    /// </summary>
    /// <param name="go">父节点对象</param>
    /// <param name="objName">需要查找的子对象名字</param>
    /// <returns></returns>
    public static Transform FindTheChildNode(GameObject go, string objName)
    {
        Transform childTransform;

        childTransform = go.transform.Find(objName);
        if (childTransform == null)
        {
            foreach (Transform item in go.transform)
            {
                childTransform = FindTheChildNode(item.gameObject, objName);
                if (childTransform != null)
                {
                    return childTransform;
                }
            }
        }
        return childTransform;
    }

    /// <summary>
    /// 获取子节点（对象）脚本
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static T GetTheChildNodeComponetScripts<T>(GameObject goParent, string childName) where T : Component
    {
        Transform searchTranformNode = null;            //查找特定子节点

        searchTranformNode = FindTheChildNode(goParent, childName);
        if (searchTranformNode != null)
        {
            return searchTranformNode.gameObject.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 给子节点添加脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">子对象名称</param>
    /// <returns></returns>
    public static T AddChildNodeCompnent<T>(GameObject goParent, string childName) where T : Component
    {
        Transform searchTranform = null;                //查找特定节点结果

        //查找特定子节点
        searchTranform = FindTheChildNode(goParent, childName);
        //如果查找成功，则考虑如果已经有相同的脚本了，则先删除，否则直接添加。
        if (searchTranform != null)
        {
            //如果已经有相同的脚本了，则先删除
            T[] componentScriptsArray = searchTranform.GetComponents<T>();
            for (int i = 0; i < componentScriptsArray.Length; i++)
            {
                if (componentScriptsArray[i] != null)
                {
                    GameObject.Destroy(componentScriptsArray[i]);
                }
            }
            return searchTranform.gameObject.AddComponent<T>();
        }
        else
        {
            return null;
        }
        //如果查找不成功，返回Null.
    }

    /// <summary>
    /// 给子节点添加父物体
    /// </summary>
    /// <param name="parent">父物体</param>
    /// <param name="child">子物体</param>
    public static void AddChildHodeToParentHode(Transform parent, Transform child)
    {
        child.SetParent(parent);
        child.localPosition = Vector3.zero;
        child.localScale = Vector3.one;
        child.localEulerAngles = Vector3.zero;
    }
}
