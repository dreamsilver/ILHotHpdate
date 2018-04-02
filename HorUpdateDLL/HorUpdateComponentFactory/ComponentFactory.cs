using System;
using System.Collections.Generic;
using HotUpdateMessage;
using UnityEngine;
using System.Reflection;

namespace HotUpdateDLL
{

    /// <summary>
    /// 组件工厂
    /// </summary>
    public class ComponentFactory : Singleton<ComponentFactory>
    {
        public enum Message
        {
            Awake,
            Start,
            Update,
            FixedUpdate,
            LateUpdate
        }


        public override void Init()
        {
            base.Init();

            #region 监听消息
            // 监听Distrot
            MessageCenter.Instance.AddListener(MessageType.DestrotGameObject, (m) =>
            {
                // 监听逻辑
                GameObject gObj = m.Content as GameObject;
                if (ReferenceLadingManager.Instance.dicScriptRefer.ContainsKey(gObj))
                {
                    // 释放对应的脚本
                    ReferenceLadingManager.Instance.dicScriptRefer[gObj] = null;
                    ReferenceLadingManager.Instance.dicScriptRefer.Remove(gObj);
                }
            });

            #endregion

        }

        private void GetUpdateOrAwakeOrStart(Message MethodName)
        {
            foreach (var item in ReferenceLadingManager.Instance.dicScriptRefer)
            {
                IsActive(item.Key);
                switch (MethodName)
                {
                    case Message.Awake:
                        break;
                    case Message.Start:
                        break;
                    case Message.Update:
                        if (isUpdata)
                            item.Value.Update();
                        break;
                    case Message.FixedUpdate:
                        if (isUpdata)
                            item.Value.FixedUpdate();
                        break;
                    case Message.LateUpdate:
                        if (isUpdata)
                            item.Value.LateUpdate();
                        break;
                    default:
                        break;
                }
            }
        }



        public void Awake()
        {

        }

        public void Start()
        {

        }

        bool isUpdata = true;

        public void Update()
        {
            GetUpdateOrAwakeOrStart(Message.Update);
        }

        public void FixedUpdate()
        {
            GetUpdateOrAwakeOrStart(Message.FixedUpdate);
        }
        public void LateUpdate()
        {
            GetUpdateOrAwakeOrStart(Message.LateUpdate);
        }


        private void IsActive(GameObject keyObj)
        {
            if (keyObj.activeInHierarchy)
            {
                isUpdata = true;
            }
            else
            {
                isUpdata = false;
            }
        }
    }
}
