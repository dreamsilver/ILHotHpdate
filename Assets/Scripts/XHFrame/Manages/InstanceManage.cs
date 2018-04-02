using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HotUpdateMessage;
using System;
using QCore;
using QCore.AssetMgr;

namespace XHFrame
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public class AssetInfo
    {
        #region AssetInfo Fields Property (资源信息字段&属性)

        private UnityEngine.Object _object; // 资源对象

        /// <summary>
        /// 资源类型
        /// </summary>
        public Type AssetType { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 参考长度
        /// </summary>
        public int RefCount { get; set; }
        #endregion

        #region IsLoaded & AssetObject Prop

        /// <summary>
        /// 是否正在加载
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return null != _object;
            }
        }
        /// <summary>
        /// 资源对象属性
        /// </summary>
        public UnityEngine.Object AssetObject
        {
            get
            {
                if (null == _object)
                {
                    _object = Resources.Load(Path);
                }
                return _object;
            }
        }




        #endregion

        #region AsyncLoad  & Load Method(异步加载 和 加载 方法);

        /// <summary>
        /// 返回协程加载对象(即时加载)
        /// </summary>
        /// <param name="loadingMethod">加载方式</param>
        /// <param name="_loaded">加载结果回调</param>
        /// <returns></returns>
        public IEnumerator GetCoroutineObject(Action<UnityEngine.Object> _loaded)
        {
            while (true)
            {
                yield return null;
                if (null == _object)
                {
                    yield return ObjectLoading();
                    yield return null;
                }
                _loaded?.Invoke(_object);
                yield break;
            }
        }
        private IEnumerator ObjectLoading()
        {


            _object = Resources.Load(Path);
            if (_object == null)
            {
                Debug.LogError($"错误 :<HotUpdateDLL.AssetInfo::ObjectLoading()> Resources Load Failure! Path:{Path}");
                yield break;
            }

        }


        /// <summary>
        /// 返回异步加载对象;
        /// </summary>
        /// <param name="loadingMethod">加载方式</param>
        /// <param name="_Loaded">加载结果回调</param>
        /// <param name="_progress">加载进度</param>
        /// <returns></returns>
        public IEnumerator GetAsyncObject(Action<UnityEngine.Object> _Loaded, Action<float> _progress)
        {
            if (_object != null)
            {
                _Loaded(_object);
                yield break;
            }
            yield return ResourcesAsyncObject(_Loaded, _progress);
        }

        private IEnumerator ResourcesAsyncObject(Action<UnityEngine.Object> _Loaded, Action<float> _progress)
        {

            ResourceRequest _resRequest = Resources.LoadAsync(Path);


            while (_resRequest.progress < 0.9)
            {
                if (null != _progress)
                {
                    _progress(_resRequest.progress);
                    yield return null;
                }
            }
            while (!_resRequest.isDone)
            {
                if (null != _progress)
                    _progress(_resRequest.progress);
                yield return null;
            }
            _object = _resRequest.asset;
            if (null != _Loaded)
            {
                _Loaded(_object);
            }
            yield return _resRequest;

        }
        #endregion
    }

    /// <summary>
    /// 实例管理
    /// </summary>
    public class InstanceManage : Singleton<InstanceManage>
    {
        private Dictionary<string, AssetInfo> dicAssetInfo = null;


        public override void Init()
        {
            dicAssetInfo = new Dictionary<string, AssetInfo>();
        }




        #region 实现实例

        /// <summary>
        /// 加载实例
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public UnityEngine.Object LoadInstance(string path)
        {
            UnityEngine.Object _obj = Load(path);
            return Instantiate(_obj);
        }

        /// <summary>
        /// 异步加载实例
        /// </summary>
        /// <param name="_Path"></param>
        /// <param name="_Loaded"></param>
        public void LoadAsyncInstance(string path, Action<UnityEngine.Object> _Loaded)
        {
            AsyncLoad(path, (_obj) => { Instantiate(_obj); });
        }
        /// <summary>
        /// 异步加载实例
        /// </summary>
        /// <param name="path"></param>
        /// <param name="_Loaded"></param>
        /// <param name="_Progress"></param>
        public void LoadAsyncInstance(string path, Action<UnityEngine.Object> _Loaded, Action<float> _Progress)
        {
            AsyncLoad(path, (_obj) => { Instantiate(_obj); }, _Progress);
        }

        #endregion

        #region 普通加载资源

        public UnityEngine.Object Load(string path)
        {
            AssetInfo assetInfo = GetAssetInfo(path);
            if (assetInfo != null)
            {
                return assetInfo.AssetObject;
            }
            return null;
        }

        #endregion

        #region 协程加载资源



        /// <summary>
        /// 协程加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="_loaded"></param>
        public void CoroutineLoad(string path, Action<UnityEngine.Object> _loaded)
        {
            AssetInfo assetInfo = GetAssetInfo(path, _loaded);
            if (assetInfo != null)
            {
                CoroutineController.Instance.StartCoroutine(assetInfo.GetCoroutineObject(_loaded));
            }
        }




        #endregion

        #region 异步加载资源

        public void AsyncLoad(string path, Action<UnityEngine.Object> _loaded)
        {
            AsyncLoad(path, _loaded, null);
        }

        public void AsyncLoad(string path, Action<UnityEngine.Object> _loaded, Action<float> _proeress)
        {
            AssetInfo _assetInfo = GetAssetInfo(path, _loaded);
            if (_assetInfo != null)
            {
                CoroutineController.Instance.StartCoroutine(_assetInfo.GetAsyncObject(_loaded, _proeress));
            }
        }


        #endregion

        #region 返回资源信息和实例对象

        #region 返回资源信息
        private AssetInfo GetAssetInfo(string path)
        {
            return GetAssetInfo(path, null);
        }

        private AssetInfo GetAssetInfo(string path, Action<UnityEngine.Object> _loaded)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Error:null _path name");
                _loaded?.Invoke(null);
                return null;
            }

            AssetInfo assetInfo = null;


            if (!dicAssetInfo.TryGetValue(path, out assetInfo))
            {
                assetInfo = new AssetInfo();
                assetInfo.Path = path;
                dicAssetInfo.Add(path, assetInfo);
            }

            assetInfo.RefCount++;
            return assetInfo;
        }


        #endregion

        #region 实例

        // 实例
        private UnityEngine.Object Instantiate(UnityEngine.Object _obj)
        {
            return Instantiate(_obj, null);
        }

        // 实例
        private UnityEngine.Object Instantiate(UnityEngine.Object _obj, Action<UnityEngine.Object> _Loaded)
        {
            UnityEngine.Object _retObj = null;
            if (_obj != null)
            {
                _retObj = MonoBehaviour.Instantiate(_obj);
                // 实例后发送消息 完成对引用管理器对热更脚本的关联
                Message message = new Message("InstanceAdCorrelationObjcet", this);
                message["gameObject"] = _retObj;
                message.Send();

                if (_retObj != null)
                {
                    if (null != _Loaded)
                    {
                        _Loaded(_retObj);
                        return null;
                    }
                    return _retObj;
                }
                else
                {
                    Debug.LogError("Error : null  Instantiate _retObj");
                    return null;
                }
            }
            else
            {
                Debug.LogError("Error : null Resources Load Return _obj");
                return null;
            }
        }
        #endregion

        #endregion
    }
}

