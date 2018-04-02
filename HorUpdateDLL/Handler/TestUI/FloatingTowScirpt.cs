using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdateDLL
{
    public class FloatingTowScirpt : BaseUI
    {
        public UnityEngine.GameObject floatingTwo;

        public override void Awake()
        {
            base.SetUIType._UIFormType = EnumUIFormType.Floating;
            base.SetUIType._FloatingUIShowPosition = EnumFloatingUIShowPosition.Right;
            base.SetUIType._UIFormShowMode = EnumUIFormShowMode.Floating;
            base.Awake();
        }

        public override EnumUIFormObject GetUIType()
        {
            throw new NotImplementedException();
        }
    }
}
