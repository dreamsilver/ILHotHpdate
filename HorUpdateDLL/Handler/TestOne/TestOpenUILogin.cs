using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdateDLL
{
    public class TestOpenUILogin : BaseComponent
    {
        public UnityEngine.GameObject login;
        public override void Start()
        {
            base.Start();
            GameObject go = login.transform.Find("Panel/Button (1)").gameObject;
            Button bu = go.GetComponent<Button>();
            bu.onClick.AddListener(() =>
            {
                //UIManager.Instance.OpenUICloesOthers(EnumUIFormObject.Loading);
                // Destroy(gameObject);
            });
        }

        public override EnumUIFormObject GetUIType()
        {
            return EnumUIFormObject.Login;
        }

    }
}
