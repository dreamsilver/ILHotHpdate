using HotUpdateMessage;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XHFrame;

namespace HotUpdateDLL
{

    /// <summary>
    /// 资源加载层
    /// </summary>
    public class ReferenceLadingManager : Singleton<ReferenceLadingManager>
    {
        public override void Init()
        {
            base.Init();
        }

        public void HandInit()
        {
            dicScriptRefer = new Dictionary<GameObject, BaseComponent>();
            InitReference();
        }


        /// <summary>
        /// 初始化引用脚本
        /// </summary>
        private void InitReference()
        {

            MessageCenter.Instance.AddListener("Reference", (m) =>
            {
                Dictionary<string, GameObject> dic = m.Content as Dictionary<string, GameObject>;
                foreach (var item in dic)
                {
                    try
                    {
                        var HotUpdateObject = CreateAScriptInstance(item.Key);
                        if (HotUpdateObject == null)
                        {
                            Debug.Log("创建热更脚本失败:" + item.Key);
                            continue;
                        }
                        HotUpdateObject.gameObject = item.Value;
                        AddReference(HotUpdateObject, dic);
                        dicScriptRefer.Add(item.Value, HotUpdateObject);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message + "没有找到相对应的热更新脚本,请核对脚本名:" + item.Key);
                        return;
                    }
                    break;

                }
            });


        }



        /// <summary>
        /// 实例unity物体引用脚本
        /// </summary>
        /// <param name="gameObj">需要实例的Unity游戏对象</param>
        private Dictionary<string, UnityEngine.GameObject> UnityReferObjectInstance(GameObject gameObj)
        {
            ReferenceManage rm = gameObj.GetComponent<ReferenceManage>();
            rm.switcher = false;
            return rm.dicRefManage;
        }

        /// <summary>
        /// 热更新实例脚本引用
        /// </summary>
        public Dictionary<GameObject, BaseComponent> dicScriptRefer = null;


        /// <summary>
        /// 創建熱更新腳本實例
        /// </summary>
        /// <param name="_namespace">需要创建实例的命名空间</param>
        /// <param name="_className">需要创建实例的类名</param>
        private BaseComponent CreateAScriptInstance(string _namespace, string _className)
        {
            string _namespaceAdClassName = _namespace + "." + _className;
            return CreateAScriptInstance(_namespaceAdClassName);
        }

        static int xx = 0; //测试创建次数

        /// <summary>
        /// 創建熱更新腳本實例
        /// </summary>
        /// <param name="_namespace">需要创建实例的命名空间</param>
        /// <param name="_className">需要创建实例的类名</param>
        private BaseComponent CreateAScriptInstance(string _className)
        {
#if DEBUG
            xx++;
            Debug.LogWarning("第" + xx + "创建" + "::::" + _className);
#endif
            Type classType = Type.GetType(_className);
            BaseComponent classObj = Activator.CreateInstance(classType) as BaseComponent;
            return classObj;
        }


        /// <summary>
        /// 获取Uinty脚本实例引用并且传递给热更新脚本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unityScript"></param>
        private void AddReference(object valus, Dictionary<string, GameObject> dicReference)
        {
            Type keyType = valus.GetType();
            FieldInfo[] keyField = keyType.GetFields();
            foreach (var item in keyField)
            {
#if DEBUG
                //Debug.Log("热更新脚本变量名 :" + item.Name);
                //foreach (var s in dicReference.Keys)
                //{
                //    Debug.Log(" 引用实例字典的名字 :" + s);
                //}
#endif
                if (!dicReference.ContainsKey(item.Name))
                    continue;
                item.SetValue(valus, dicReference[item.Name]);
            }
            MessageCenter.Send("TestReference", this);
        }

        // 返回脚本字段
        private FieldInfo[] GetReference(object script)
        {
            Type st = script.GetType();
            FieldInfo[] sField = st.GetFields();
            return sField;
        }

    }
}

