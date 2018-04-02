using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdateDLL
{
    public class TestDynamicLoadingReference : BaseComponent
    {
        public GameObject cube;
        public GameObject sphere;
        bool sw = true;
        public TestDynamicLoadingReference()
        {
            //HotUpdateMessage.MessageCenter.Instance.AddListener("TestReference", (m) =>
            //{
            //    if (sw)
            //    {
            //        Debug.Log("我是以获取的引用" + cube.name);
            //        Debug.Log("我是以获取的引用" + sphere.name);
            //        Debug.Log("游戏对象是:" + gameObject);
            //        sw = false;
            //    }

            //});
        }


        Vector3 v;

        public override void Start()
        {
            base.Start();
            v = UnityEngine.Random.insideUnitSphere * 10;
            Debug.Log(v);
            Debug.Log("<color=red>" + "初始化" + sphere + "</color>");
        }

        public override void Update()
        {
            gameObject.transform.position = v;
            Debug.Log(this.GetHashCode());
        }

        public override EnumUIFormObject GetUIType()
        {
            throw new NotImplementedException();
        }
    }
}
