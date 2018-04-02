using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdateDLL
{
    public class LittleWindewScirpt : BaseUI
    {
        public UnityEngine.GameObject littleWindew;
        public override void Start()
        {
            base.SetUIType._UIFormShowMode = EnumUIFormShowMode.ReversChange;
            base.SetUIType._UIFormType = EnumUIFormType.PopUp;
            base.SetUIType._UIFormLucancyType = EnumUIFormLucencyType.ImPenetrable;
            base.Start();
            Debug.Log(littleWindew);
            ButtonCilck("END", () =>
            {
                CloseUI();
            });
        }
        public override EnumUIFormObject GetUIType()
        {
            throw new NotImplementedException();
        }
        public override void Update()
        {
            base.Update();
        }
    }
}
