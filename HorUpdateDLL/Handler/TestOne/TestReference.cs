using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdateDLL
{
    public class TestReference : BaseComponent
    {
        public GameObject directionalLight;
        bool b = true;
        public TestReference()
        {
            HotUpdateMessage.MessageCenter.Instance.AddListener("TestReference", (m) =>
            {
                if (b)
                {
                    Debug.Log("我是以获取的引用" + directionalLight.name);
                    Debug.Log("游戏对象是:" + gameObject);
                    b = false;
                }

            });
        }

        public override EnumUIFormObject GetUIType()
        {
            throw new NotImplementedException();
        }
    }
}
