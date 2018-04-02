using HotUpdateMessage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using LitJson;
using XHFrame;

namespace HotUpdateDLL
{
    public class ConfigFileData : Singleton<ConfigFileData>
    {
        public TextAsset uIPath;
        public ConfigFileData()
        {
            MessageCenter.Instance.AddListener("TextAssetData", m =>
            {
                ConfigFlieData configFlieData = m.Sender as ConfigFlieData;
                JsonToObject(configFlieData.jsonConfigData);
                CsvToObject(configFlieData.csvConfigData);
            });
        }

        private void JsonToObject(List<ScenesConfigData> jsList)
        {
            foreach (var item in jsList)
            {
                string objName = item.DataToObjectNmae;
                switch (objName)
                {
                    case "UIPathConfigFiles":
                        UIPathConfigFiles uIPathConfigFiles = JsonMapper.ToObject<UIPathConfigFiles>(item.ConfigData.text.ToString());
                        foreach (var data in uIPathConfigFiles.listPath)
                        {
                            UIManager.Instance.dicFormPath.Add(data.Name, data.Path);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void CsvToObject(List<ScenesConfigData> csvList)
        {

        }

        public void Start()
        {

        }
    }
}
