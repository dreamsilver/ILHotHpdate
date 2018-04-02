using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdateDLL
{
    public class HomeSciprt : BaseUI
    {
        public UnityEngine.GameObject home;

        public override EnumUIFormObject GetUIType()
        {
            throw new NotImplementedException();
        }

        public HomeSciprt()
        {

        }
        public override void Start()
        {
            SetUIType._UIFormType = EnumUIFormType.Normal;
            SetUIType._UIFormShowMode = EnumUIFormShowMode.HideOther;
            Debug.Log(SetUIType._UIFormShowMode);
            base.Start();
            Debug.Log(home);
            Button bu = home.transform.Find("TestPop").GetComponent<Button>();
            Button bu2 = home.transform.Find("End").GetComponent<Button>();
            bu.onClick.AddListener(() => { UIManager.Instance.OpenUI("PopOne"); });
            bu2.onClick.AddListener(() => { UIManager.Instance.CloseUI("Home"); });
            ButtonCilck("TestPop2", () =>
            {
                OpenUI("FloatingUI");
            });
            ButtonCilck("TestPop3", () =>
            {
                OpenUI("FloatingUI");
            });
            ButtonCilck("Game1", () =>
            {
                OpenUI("FloatingUI");
            });
            ButtonCilck("Game2", () =>
            {
                OpenUI("FloatingUITwo");
            });
            ButtonCilck("Game3", () =>
            {
                OpenUI("FloatingUITwo");
            });
        }
        public override void Update()
        {
            base.Update();
            if (Input.GetMouseButton(0))
            {
                CloseUI("FloatingUI");
            }
        }
    }
}
