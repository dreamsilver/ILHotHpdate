using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdateDLL
{
    public class Test : BaseComponent
    {
        public UnityEngine.GameObject text;
        public UnityEngine.GameObject inputField;
        public UnityEngine.GameObject button_1_;
        public UnityEngine.GameObject button;

        public bool sw = true;
        public Test()
        {
            HotUpdateMessage.MessageCenter.Instance.AddListener("TestReference", (m) =>
            {
                if (sw)
                {
                    Debug.Log("我是以获取的引用" + text.name);
                    Debug.Log("我是以获取的引用" + inputField.name);
                    Debug.Log("我是以获取的引用" + button_1_.name);
                    Debug.Log("我是以获取的引用" + button.name);
                    Debug.Log("游戏对象是:" + gameObject);
                    sw = false;
                }
            });
        }

        public override EnumUIFormObject GetUIType()
        {
            return EnumUIFormObject.Loading;
        }
    }
}
