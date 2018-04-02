using System;
using System.Collections.Generic;
using UnityEngine;

namespace XHFrame
{
    public class DDOLSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _Instance = null;

        public static T Instance
        {
            get
            {
                if (null == _Instance)
                {
                    GameObject go = GameObject.Find("DDOLSingleton");
                    if (null == go)
                    {
                        go = new GameObject("DDOLSingleton");
                        DontDestroyOnLoad(go);
                    }
                    _Instance = go.AddComponent<T>();
                }

                return _Instance;
            }
        }

        private void Awake()
        {
            Init();
        }

        private void OnApplicationQuit()
        {
            _Instance = null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {

        }
    }

}
