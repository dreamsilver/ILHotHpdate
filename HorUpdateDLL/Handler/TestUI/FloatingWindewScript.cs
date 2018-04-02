using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdateDLL
{
    public class FloatingWindewScript : BaseUI
    {
        public UnityEngine.GameObject floatingWindew;

        public override void Awake()
        {
            base.SetUIType._FloatingUIShowPosition = EnumFloatingUIShowPosition.Below;
            base.SetUIType._UIFormShowMode = EnumUIFormShowMode.Floating;
            base.SetUIType._UIFormType = EnumUIFormType.Floating;
            base.Awake();
        }

        public override EnumUIFormObject GetUIType()
        {
            throw new NotImplementedException();
        }
    }
}
