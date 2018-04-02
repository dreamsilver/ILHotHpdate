using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdateDLL
{
    public class TestOpenUILaging : BaseComponent
    {
        public UnityEngine.GameObject loading;

        static int i = 0;
        public override void Start()
        {
            base.Start();
            GameObject go = loading.transform.Find("Panel/Button").gameObject;
            Button bu = go.GetComponent<Button>();
            bu.onClick.AddListener(() =>
            {
              //  UIManager.Instance.OpenUICloesOthers(EnumUIFormObject.Login);
            });
        }

        public override EnumUIFormObject GetUIType()
        {
            return EnumUIFormObject.Loading;
        }
    }
}
