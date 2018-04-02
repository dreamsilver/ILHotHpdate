using HotUpdateMessage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HotUpdateDLL
{
    public abstract class BaseComponent
    {

        #region 缓存 GameObject & Transform

        // 缓存游戏物体
        public GameObject gameObject;

        //是否禁用脚本  默认开启
        public bool enable = true;

        // 缓存Transform
        private Transform cacheTransform;

        /// <summary>
        /// 缓存Transform
        /// </summary>
        public Transform CacheTransform
        {
            get
            {
                if (cacheTransform == null)
                    cacheTransform = gameObject.transform;
                return cacheTransform;
            }
        }

        #endregion

        #region 提供Unity基础方法
        /// <summary>
        /// 删除游戏物体
        /// </summary>
        /// <param name="gameObject">需要删除的对象</param>
        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
            // 通知工厂 销毁当前实例 
            MessageCenter.Send(MessageType.DestrotGameObject, this, gameObject);
            state = EnumObjectState.None;
        }

        /// <summary>
        /// 删除游戏物体
        /// </summary>
        /// <param name="gameObject"> 需要删除的对象</param>
        /// <param name="time"> 延迟多少秒 </param>
        public void Destroy(GameObject gameObject, float time)
        {
            GameObject.Destroy(gameObject, time);
            MessageCenter.Send(MessageType.DestrotGameObject, this, gameObject);
            state = EnumObjectState.None;
        }

        /// <summary>
        /// 删除游戏物体,并且从内存中移除
        /// </summary>
        /// <param name="gameObject">需要删除的游戏物体 </param>
        public void DestroyImmediate(GameObject gameObject)
        {
            GameObject.DestroyImmediate(gameObject);
            MessageCenter.Send(MessageType.DestrotGameObject, this, gameObject);
            state = EnumObjectState.None;
        }


        #endregion

        /// <summary>
        /// 返回UI类型(抽象方法,重写该方法 返回脚本类型  // 例:retrun EnumUIFormObject.Loading)
        /// </summary>
        /// <returns></returns>
        public abstract EnumUIFormObject GetUIType();

        /// <summary>
        /// UI对象类型
        /// </summary>
        private EnumObjectState state = EnumObjectState.None;

        /// <summary>
        /// 当UI状态改变时
        /// </summary>
        public event StateChangedEvent OnStateChanged;


        /// <summary>
        /// UI状态
        /// </summary>
        public EnumObjectState State
        {
            protected get
            {
                return state;
            }
            set
            {
                EnumObjectState oldState = state; // 保存之前的UI类型
                state = value;
                OnStateChanged?.Invoke(this, state, oldState);
            }
        }

        public BaseComponent()
        {
            Init();
            MessageCenter.Instance.AddListener("TestReference", (m) =>
            {
                Awake();
                Start();
                MessageCenter.Instance.RemoveListener("TestReference", (r) => { });
            });
        }

        public virtual void Init()
        {
            Debug.Log("<color=blue>我创建了一个基础类</color>");
            state = EnumObjectState.Initial;
        }

        public virtual void Awake()
        {
            State = EnumObjectState.Loading;
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {

        }

        public virtual void FixedUpdate()
        {

        }
        public virtual void LateUpdate()
        {

        }



        /// <summary>
        /// 释放UI对象
        /// </summary>
        void Release()
        {
            State = EnumObjectState.Closing;
            Destroy(gameObject);
        }


        private void OnDestroy()
        {
            State = EnumObjectState.None;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void OnRelease()
        {
            State = EnumObjectState.None;
            // 关闭声音
            OnPlayCloseUIAudio();
            Release();
        }

        public void OnClosing()
        {
            State = EnumObjectState.Closing;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected virtual void OnLoadData() { }

        /// <summary>
        /// 打开音乐
        /// </summary>
        protected virtual void OnPlayOpenUIAudio() { }

        /// <summary>
        /// 关闭音乐
        /// </summary>
        protected virtual void OnPlayCloseUIAudio() { }

        /// <summary>
        /// 设置当UI打开时
        /// </summary>
        /// <param name="UIparams"></param>
        public void SetUIWhenOpening(params object[] UIparams)
        {
            SetUI(UIparams);
            LoadDataAsyn();
        }

        /// <summary>
        /// 设置UI参数
        /// </summary>
        /// <param name="uiParams"></param>
        public virtual void SetUIParam(params object[] uiParams)
        {

        }

        /// <summary>
        /// 设置UI
        /// </summary>
        /// <param name="UIparams"></param>
        protected virtual void SetUI(params object[] UIparams)
        {
            State = EnumObjectState.Loading;
        }

        /// <summary>
        /// 异步加载改变状态
        /// </summary>
        /// <returns></returns>
        private async void LoadDataAsyn()
        {
            await Task.Run(() =>
            {
                new WaitForSeconds(0);
                if (State == EnumObjectState.Loading)
                {
                    OnLoadData();
                    State = EnumObjectState.Ready;
                }
            });
        }
    }
}
