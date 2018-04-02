using QCore;
using QCore.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace QCore.AssetMgr
{
    /// <summary>
    /// AB包管理类
    /// </summary>
    public class ABMgr : Core.Singleton<ABMgr>, IDisposable
    {
        /// <summary>
        /// AB包缓存
        /// </summary>
        private readonly Dictionary<string, AssetBundleInfo> cacheAB = new Dictionary<string, AssetBundleInfo>();

        /// <summary>
        /// 主要的清单文件
        /// </summary>
        private AssetBundleManifest manifest;

        public AssetBundleManifest Manifest
        {
            get
            {
                return manifest;
            }
        }

        private ABMgr() { }

        /// <summary>
        /// 加载主要的Manifest文件
        /// </summary>
        /// <returns></returns>
        public IEnumerator LoadManifest()
        {
            if (this.manifest != null)
                yield break;

            yield return LoadABInfo(PathUtils.GetRuntimePlatform(), abInfo =>
            {
                this.manifest = abInfo.AB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                Debug.Log("load main manifest ");
                abInfo.Dispose();
            });
        }

        /// <summary>
        /// 加载一个资源
        /// </summary>
        /// /// <param name="abName">ab包名</param>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public IEnumerator LoadAsset(string abName, string assetName, Action<UnityEngine.Object> action)
        {
            Debug.Log(abName);
            if (string.IsNullOrEmpty(abName) && string.IsNullOrEmpty(assetName))
                yield break;

            abName = abName + ".assetbundle";

            UnityEngine.Object objTemp = LoadAssetByCache(abName, assetName);

            if (objTemp != null)
            {
                action(objTemp);
                Debug.Log($"load asset:{abName}/{assetName} by cache!");
                yield break;
            }

            if (this.manifest == null)
            {
                Debug.LogError("main manifest is null!");
                yield break;
            }

            // 加载依赖
            yield return LoadDependencies(abName);

            yield return LoadABInfo(abName, abInfo =>
            {
                UnityEngine.Object obj = abInfo.LoadAsset(assetName);
                action(obj);
            });
        }

        /// <summary>
        /// 从缓存中获取资源对象
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public UnityEngine.Object LoadAssetByCache(string abName, string assetName)
        {
            if (!this.cacheAB.ContainsKey(abName))
            {
                return null;
            }
            AssetBundleInfo info = this.cacheAB[abName];
            return info.LoadAsset(assetName);
        }

        /// <summary>
        /// 加载AB包的依赖
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public IEnumerator LoadDependencies(string abName)
        {
            // 获取依赖包
            string[] names = this.manifest.GetAllDependencies(abName);
            foreach (string name in names)
            {
                // 什么都不用做，LoadABAsync会缓存。
                yield return LoadABInfo(name, abInfo => { });
            }
        }

        /// <summary>
        /// 加载ABInfo
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator LoadABInfo(string abName, Action<AssetBundleInfo> callback)
        {
            AssetBundleInfo abInfo = null;

            // 查询缓存
            if (cacheAB.ContainsKey(abName))
            {
                abInfo = cacheAB[abName];
                Debug.Log($"load cache ab {abName}");
            }
            else
            {
                // 获取AB包在各个平台上的路径
                string abPath = PathUtils.GetAssetsBundlePath(abName);
                yield return LoadAB(abPath, ab =>
                 {
                     if (cacheAB.ContainsKey(abName))
                     {
                         abInfo = cacheAB[abName];
                         abInfo.AB = ab;
                     }
                     else
                     {
                         abInfo = new AssetBundleInfo(ab);
                         cacheAB.Add(abName, abInfo);
                     }
                 });
            }
            callback(abInfo);
        }

        /// <summary>
        /// 加载一个AB包
        /// </summary>
        /// <param name="abPath"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator LoadAB(string abPath, Action<AssetBundle> callback)
        {
            UnityWebRequest req = UnityWebRequest.GetAssetBundle(abPath);
            yield return req.Send();

            if (req.isNetworkError)
            {
                Debug.LogError(req.error);
                yield break;
            }

            AssetBundle ab = (req.downloadHandler as DownloadHandlerAssetBundle)?.assetBundle;
            if (ab == null)
            {
                Debug.LogError("loading error:" + abPath);
                yield break;
            }
            Debug.Log($"load ab {abPath}");
            callback(ab);
            req.Dispose();
        }

        public void Dispose()
        {
            foreach (AssetBundleInfo abInfo in cacheAB.Values)
            {
                abInfo.Dispose();
            }
        }
    }
}