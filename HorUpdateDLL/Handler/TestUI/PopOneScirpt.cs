using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdateDLL
{
    public class PopOneScirpt : BaseUI
    {
        public UnityEngine.GameObject popOne;

        public override EnumUIFormObject GetUIType()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            base.SetUIType._UIFormShowMode = EnumUIFormShowMode.ReversChange;
            base.SetUIType._UIFormType = EnumUIFormType.PopUp;
            base.SetUIType._UIFormLucancyType = EnumUIFormLucencyType.ImPenetrable;
            base.Start();
            Debug.Log(popOne);
            Button button = popOne.transform.Find("Test1").GetComponent<Button>();
            Button button2 = popOne.transform.Find("End").GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                UIManager.Instance.OpenUI("LittleWindew");
            });
            button2.onClick.AddListener(() =>
            {
                UIManager.Instance.CloseUI("PopOne");
            });
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
