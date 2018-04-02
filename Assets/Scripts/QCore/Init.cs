using QCore.AssetMgr;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace QCore
{
    /// <summary>
    /// 程序入口，初始化类
    /// </summary>
    public class Init : MonoBehaviour
    {
        public Text debugText;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Debugger.Instance.text = debugText;
            Debugger.Instance.Init();

            StartCoroutine(LoadInitAsset());

        }

        IEnumerator LoadInitAsset()
        {
            yield return ABMgr.Instance.LoadManifest();
            yield return ABMgr.Instance.LoadAsset("hotscript", "HotUpdateDll.bytes", (a) =>
            {
                if (a is TextAsset)
                {
                    Debug.Log("加载【HotUpdateDll】成功！" + (a as TextAsset).bytes[0]);
                }
            });
            ABMgr.Instance.Dispose();
        }
    }
}
