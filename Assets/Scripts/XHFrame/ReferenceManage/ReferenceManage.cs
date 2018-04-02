using System.Collections.Generic;
using UnityEngine;
using HotUpdateMessage;
using System;

namespace XHFrame
{
    [Serializable]
    public class ReferenceData
    {
        public string name;
        public UnityEngine.Object Object;
        public bool deleteField = false;
    }
    /// <summary>
    /// 引用管理器
    /// </summary>
    public class ReferenceManage : MonoBehaviour
    {
        private string HorUpdateScriptnNamespace = Defines.HorUpdateScriptnNamespace;


        /// <summary>
        /// 需要同步的热更新脚本
        /// </summary>
        public string ButtJoinHorUpdateScript;

        public string path = System.IO.Directory.GetCurrentDirectory() + @"\HorUpdateDLL\Handler";

        /// <summary>
        /// 引用名称
        /// </summary>
        public List<ReferenceData> objectList = new List<ReferenceData>();

        /// <summary>
        /// 引用字典
        /// </summary>
        public Dictionary<string, GameObject> dicRefManage = new Dictionary<string, GameObject>();

        public Dictionary<string, UnityEngine.Object> testDir = new Dictionary<string, UnityEngine.Object>();

        /// <summary>
        /// 是否允许发送脚本实例消息
        /// </summary>
        public bool switcher = true;

        bool isGameObject = true;

        private void Awake()
        {

            HorUpdateScriptnNamespace = Defines.HorUpdateScriptnNamespace;
            // 初始化发送引用
            AddReference();
            if (isGameObject)
                if (switcher)
                    MessageCenter.Send("Reference", this, dicRefManage);
        }

        private void Start()
        {

        }

        private void OnValidate()
        {

        }

        private void AddReference()
        {

            dicRefManage.Clear();
            dicRefManage.Add(HorUpdateScriptnNamespace + "." + ButtJoinHorUpdateScript, this.gameObject);
            //Debug.Log(objectList.Count);
            for (int i = 0; i < objectList.Count; i++)
            {
                if (objectList[i].Object == null)
                    continue;
                try
                {
                    if (objectList[i].Object is GameObject)
                    {
                        dicRefManage.Add(objectList[i].name, objectList[i].Object as GameObject);
                        isGameObject = true;
                    }
                    else if (objectList[i].Object is TextAsset)
                    {
                        //Message message = new Message("TextAssetData", this);
                        //message["Name"] = objectList[i].name;
                        //message["Value"] = objectList[i].Object;
                        //message.Send();


                        isGameObject = false;
                    }
                    //Debug.Log("字典的值:" + dicRefManage[objectList[i].name]);
                }
                catch (Exception)
                {
                    Debug.LogError("引用保存失败,您的引用中有相同的引用名称, 请修改引用");
                    return;
                }

            }
        }

    }
}

