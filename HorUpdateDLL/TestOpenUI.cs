using HotUpdateDLL;
using HotUpdateMessage;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XHFrame;

namespace HotUpdateDLL
{
    public class TestOpenUI
    {
        public static void Update()
        {
            ComponentFactory.Instance.Update();
        }

        static Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();

        public static void ThisUI()
        {
            ReferenceLadingManager.Instance.HandInit();
            ConfigFileData.Instance.Start();
            MessageCenter.Instance.AddListener("BatchABLoad", m =>
            {
                UIManager.Instance.dicBatchLoadUI = m.Content as Dictionary<string, GameObject>;
                foreach (var item in UIManager.Instance.dicBatchLoadUI.Values)
                {
                    Debug.Log(item);
                }

                //dic = m.Content as Dictionary<string, GameObject>;
                //foreach (var item in dic)
                //{
                //    var go = GameObject.Instantiate(item.Value);
                //    go.name = item.Value.name;
                //    go.SetActive(false);
                //    UIManager.Instance.dicBatchLoadUI.Add(item.Key, go);
                //}
                UIManager.Instance.OpenUI("Login");
            });

            //PathDic pathDic = new PathDic();
            //pathDic.Name = "Canvas";
            //pathDic.Path = "Scnene";

            //PathDic pathDic1 = new PathDic();
            //pathDic1.Name = "Canvas1";
            //pathDic1.Path = "Scnene1";

            //UIPathConfigFiles uIPathConfigFiles = new UIPathConfigFiles();
            //uIPathConfigFiles.listPath.Add(pathDic);
            //uIPathConfigFiles.listPath.Add(pathDic1);

            //JsonData str = JsonMapper.ToJson(uIPathConfigFiles);
            //Debug.Log(str.ToString());

            //Dictionary<string, string> dicTestPath = new Dictionary<string, string>();
            //dicTestPath.Add("Login", "scene1/normal/Login");
            //dicTestPath.Add("Loading", "scene1/normal/Loading");
            //dicTestPath.Add("Home", "scene1/normal/Home");
            //dicTestPath.Add("PopOne", "scene1/popUP/PopOne");

            ABLoadMessager.BatchABLoad(UIManager.Instance.dicFormPath);

        }
    }


}
