using QCore.AssetMgr;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HotUpdateMessage;

namespace XHFrame
{
    /// <summary>
    ///加载AB包
    /// </summary>
    public static class ABLoadMessager
    {
        public static void ABLoad(string path, Action<UnityEngine.Object> a)
        {
            CoroutineController.Instance.StartCoroutine(AssetBundleLoad(path, a));
        }

        /// <summary>
        /// 批量加载AB包;
        /// </summary>
        /// <param name="dir"></param>
        public static void BatchABLoad(Dictionary<string, string> dir)
        {
            CoroutineController.Instance.StartCoroutine(BatchAssetBundleLoad(dir));
        }

        #region 私有方法

        private static IEnumerator AssetBundleLoad(string path, Action<UnityEngine.Object> a)
        {
            string[] abPath = PathToABname(path);
            yield return ABMgr.Instance.LoadAsset(abPath[0], abPath[1], a);
        }

        private static IEnumerator BatchAssetBundleLoad(Dictionary<string, string> dir)
        {
            yield return ABMgr.Instance.LoadManifest();
            Dictionary<string, GameObject> dicData = new Dictionary<string, GameObject>();
            foreach (var item in dir)
            {
                string[] abPath = PathToABname(item.Value);
                yield return ABMgr.Instance.LoadAsset(abPath[0], abPath[1], obj =>
                {
                    dicData.Add(item.Key, obj as GameObject);
                    Message message = new Message("InstanceAdCorrelationObjcet", "ABLoadMessager");
                    message["gameObject"] = obj;
                    message.Send();
                });
            }
            MessageCenter.Send("BatchABLoad", "BatchAssetBundleLoad", dicData);

        }


        /// <summary>
        /// 分割路径字符
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        private static string[] PathToABname(string Path)
        {
            string[] abNameAndPrefadName = Path.Split('/');
            string[] abAndPrefad = new string[2];
            if (abNameAndPrefadName.Length > 3)
            {
                Debug.LogWarning("InstanceManage::PathToABname() >> 路径名不符合转换");
                return null;
            }
            if (abNameAndPrefadName.Length > 2)
            {
                abAndPrefad[0] = abNameAndPrefadName[0] + "/" + abNameAndPrefadName[1];
                abAndPrefad[1] = abNameAndPrefadName[2];
            }
            else if (abNameAndPrefadName.Length == 2)
            {
                abAndPrefad = abNameAndPrefadName;
            }
            return abAndPrefad;
        }
        #endregion

    }
}
