using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdateDLL
{
    public class Abccc : BaseComponent
    {
        public UnityEngine.GameObject capsule;
        public UnityEngine.GameObject sphere;
        public UnityEngine.GameObject cube;


        public bool sw = true;
        public Abccc()
        {
            HotUpdateMessage.MessageCenter.Instance.AddListener("TestReference", (m) =>
            {
                Debug.Log("我是以获取的引用" + capsule.name);
                Debug.Log("我是以获取的引用" + sphere.name);
                Debug.Log("我是以获取的引用" + cube.name);
                Debug.Log("游戏对象是:" + gameObject);
                sw = false;
            });
        }

        public override EnumUIFormObject GetUIType()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            Debug.Log("222");
        }

        public override void Update()
        {

            Debug.Log("Update");
        }

    }
}
