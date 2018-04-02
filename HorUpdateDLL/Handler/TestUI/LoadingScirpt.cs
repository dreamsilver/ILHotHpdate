using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdateDLL
{
    public class LoadingScirpt : BaseUI
    {
        public UnityEngine.GameObject loading;

        public override void Start()
        {
            base.Start();
            Debug.Log(loading);
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
