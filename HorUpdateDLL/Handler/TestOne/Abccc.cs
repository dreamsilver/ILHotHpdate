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
                Debug.Log("�����Ի�ȡ������" + capsule.name);
                Debug.Log("�����Ի�ȡ������" + sphere.name);
                Debug.Log("�����Ի�ȡ������" + cube.name);
                Debug.Log("��Ϸ������:" + gameObject);
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
