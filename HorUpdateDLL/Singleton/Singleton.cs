using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdateDLL
{
    public class Singleton<T> where T :class , new()
    {
        private static T _instance = null;

        public static T Instance {
            get {
                if (null == _instance)
                    _instance = new T();
                return _instance;
            }

        }

        protected Singleton()
        {
            if (_instance != null)
                Debug.LogError("This" + (typeof(T)).ToString() + "Singleton Instance is not null !!!!");
            Init();
        }

        public virtual void Init()
        {
        }
    }
}
