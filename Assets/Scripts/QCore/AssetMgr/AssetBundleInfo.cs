using System;
using System.Collections.Generic;
using UnityEngine;
namespace QCore.AssetMgr
{
    /// <summary>
    /// AB包信息
    /// </summary>
    public class AssetBundleInfo : IDisposable
    {
        #region 字段
        /// <summary>
        /// AB包
        /// </summary>
        private AssetBundle ab;

        /// <summary>
        /// 当前AB包中的缓存
        /// </summary>
        private readonly Dictionary<string, UnityEngine.Object> cacheObject = new Dictionary<string, UnityEngine.Object>();

        /// <summary>
        /// 引用计数
        /// </summary>
        private int refCount = 0;

        #endregion

        #region 属性
        public AssetBundle AB
        {
            get
            {
                return this.ab;
            }
            set
            {
                this.ab = value;
            }
        }
        #endregion

        #region 资源对象缓存操作
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public UnityEngine.Object this[string assetName]
        {
            get
            {
                return cacheObject[assetName];
            }
        }

        /// <summary>
        /// 从AB包中加载一个资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string assetName)
        {
            if (cacheObject.ContainsKey(assetName))
            {
                return this[assetName];
            }

            UnityEngine.Object obj = null;
            if (ab != null)
            {
                obj = ab.LoadAsset(assetName);
                cacheObject.Add(assetName, obj);
            }
            return obj;
        }

        /// <summary>
        /// 移除一个缓存中的资源
        /// </summary>
        /// <param name="assetName"></param>
        public void RemoveCacheAsset(string assetName)
        {
            if (!string.IsNullOrEmpty(assetName) && cacheObject.ContainsKey(assetName))
            {
                Resources.UnloadAsset(cacheObject[assetName]);
                cacheObject.Remove(assetName);
            }
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void ClearCache()
        {
            cacheObject.Clear();
        }

        /// <summary>
        /// 包含指定Key
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool ContainsKey(string assetName)
        {
            return cacheObject.ContainsKey(assetName);
        }

        #endregion

        public AssetBundleInfo(AssetBundle ab)
        {
            this.ab = ab;
            refCount = 0;
        }

        #region 引用管理、资源释放 
        /// <summary>
        /// 增加引用
        /// </summary>
        public void AddRefCount()
        {
            refCount++;
        }

        /// <summary>
        /// 减少引用
        /// </summary>
        public void SubRefCount()
        {
            refCount--;
            if (refCount == 0)
            {
                ab.Unload(true);
                ab = null;
            }
        }

        public void Dispose()
        {
            if (ab != null)
            {
                Debug.Log("unload ab:" + ab.name);
                ab.Unload(false);
                ab = null;
            }
        }
        #endregion
    }
}
