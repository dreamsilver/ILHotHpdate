using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdateDLL
{
    public class LoginScirpt : BaseUI
    {
        public UnityEngine.GameObject login;

        public override EnumUIFormObject GetUIType()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            base.Start();
            Debug.Log(login);
            Button bu = login.transform.Find("LoginButton").GetComponent<Button>();
            bu.onClick.AddListener(() =>
            {
                UIManager.Instance.OpenUI("Home");
            });
        }
        public override void Update()
        {
            base.Update();
        }
    }
}
