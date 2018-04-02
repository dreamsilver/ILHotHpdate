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
                    Debug.Log("�����Ի�ȡ������" + text.name);
                    Debug.Log("�����Ի�ȡ������" + inputField.name);
                    Debug.Log("�����Ի�ȡ������" + button_1_.name);
                    Debug.Log("�����Ի�ȡ������" + button.name);
                    Debug.Log("��Ϸ������:" + gameObject);
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
