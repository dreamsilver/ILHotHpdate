using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using HotUpdateMessage;

namespace XHFrame
{
    public class ConfigFlieData : MonoBehaviour
    {
        public List<ScenesConfigData> jsonConfigData;
        public List<ScenesConfigData> csvConfigData;

        private void Awake()
        {
            MessageCenter.Send("TextAssetData", this);
        }
    }

    [Serializable]
    public class ScenesConfigData
    {
        public string ScenesName;
        /// <summary>
        /// 信息转换对象的对象名
        /// </summary>
        public string DataToObjectNmae;
        public TextAsset ConfigData;
    }



}
