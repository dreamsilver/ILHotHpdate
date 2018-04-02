using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using QCore.AssetMgr;
using System;
using System.Collections.Generic;
using QCore.HttpNet;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace QCore.AssetMgr
{
    /// <summary>
    /// 资源更新
    /// </summary>
    public class ResourceUpdate : MonoBehaviour
    {
        /// <summary>
        /// 资源服务器地址
        /// </summary>
        public string resServerUrl = "";
        /// <summary>
        /// 本地版本号
        /// </summary>
        public float localResVersion;
        /// <summary>
        /// 远程版本号
        /// </summary>
        public float remoteResVersion;

        /// <summary>
        /// 远程版本文件路径
        /// </summary>
        public string remoteVersionPath;

        public Slider slider;

        /// <summary>
        /// 本地Manifest文件
        /// </summary>
        private AssetBundleManifest localManifest;

        /// <summary>
        /// 远程Manifest文件
        /// </summary>
        private AssetBundleManifest remoteManifest;

        /// <summary>
        /// 需要下载的文件队列
        /// </summary>
        private List<string> needDownloadList = new List<string>();

        /// <summary>
        /// 已经下载完成的资源文件 K:ABName V:缓存中的路径
        /// </summary>
        private Dictionary<string, string> cacheABPath = new Dictionary<string, string>();

        /// <summary>
        /// 从服务器下载后的Manifest文件在缓存中的路径
        /// </summary>
        private string manifestCachePath;

        /// <summary>
        /// 是否下载完成
        /// </summary>
        private bool isDnoe;

        /// <summary>
        /// 加载进度
        /// </summary>
        private float process;

        void Start()
        {
            StartCoroutine(CheckVersion());
        }

        void Update()
        {
            if (!isDnoe)
            {
                slider.value = process;
            }
            else
            {
                slider.value = 1;
            }
        }

        /// <summary>
        /// 验证本地资源版本和服务器资源版本
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckVersion()
        {
            remoteVersionPath = resServerUrl + "Windows/version.txt";
            var localVersionPath = PathUtils.GetFilePath("version.txt");
            var isLocalVersionError = false;
            //Debug.Log(remoteVersionPath);

            // 加载本地版本号
            yield return LoadVersion(localVersionPath, (e, v) =>
            {
                if (e != null)
                {
                    Debug.LogError("加载本地版本号错误：" + e);
                    isLocalVersionError = true;
                    return;
                }

                localResVersion = Convert.ToSingle(v);
            });

            if (isLocalVersionError)
                yield break;

            // 加载远程版本号
            yield return LoadVersion(remoteVersionPath, (e, v) =>
            {
                if (e != null)
                {
                    Debug.LogError("加载远程版本号错误：" + e);
                    return;
                }
                remoteResVersion = Convert.ToSingle(v);
            });

            // 无法获取服务器资源版本号
            if (remoteResVersion == 0)
            {
                // 无法连接到服务器
                Debug.Log("无法从服务器获取资源版本号！");
                yield break;
            }

            // 判断是否需要更新资源
            if (remoteResVersion == localResVersion)
            {
                Debug.Log("不需要更新资源！");
                yield break;
            }
            Debug.Log("开始更新资源");
            yield return CompareLocalRes();
            // 分析需要下载的资源
            ResDownLoadProfilter(localManifest, remoteManifest);
            // 下载资源
            DownLoadResources(needDownloadList);
        }

        /// <summary>
        /// 和服务器比较本地资源
        /// </summary>
        /// <returns></returns>
        public IEnumerator CompareLocalRes()
        {
            // 加载本地Manifest文件
            yield return ABMgr.Instance.LoadManifest();
            this.localManifest = ABMgr.Instance.Manifest;

            // 路径是Windows/Widnows AssetBundle包
            var remoteManifestPath = resServerUrl + PathUtils.GetRuntimePlatform() + "/" + PathUtils.GetRuntimePlatform();

            // 加载远程Manifest文件
            yield return ABMgr.Instance.LoadAB(remoteManifestPath, ab =>
             {
                 this.remoteManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
             });

            if (localManifest == null || remoteManifest == null)
            {
                Debug.LogError("加载Manifest文件失败！");
                yield break;
            }
        }

        /// <summary>
        /// Manifest文件分析出，需要更新的文件名称。
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void ResDownLoadProfilter(AssetBundleManifest a, AssetBundleManifest b)
        {
            // 本地
            Dictionary<string, Hash128> aDir = new Dictionary<string, Hash128>();

            // 远程
            Dictionary<string, Hash128> bDir = new Dictionary<string, Hash128>();

            GetHash128ToDicByAssetBundleManifest(a, aDir);
            GetHash128ToDicByAssetBundleManifest(b, bDir);

            foreach (var item in bDir)
            {
                // 本地不存在此资源，就直接添加进下载列表。
                if (!aDir.ContainsKey(item.Key))
                {
                    needDownloadList.Add(item.Key);
                    continue;
                }

                // 对比本地和远程
                Hash128 aHash = aDir[item.Key];
                if (aHash != item.Value)
                {
                    needDownloadList.Add(item.Key);
                }
            }
        }

        /// <summary>
        /// 获取AssetBundleHash 到字典中
        /// </summary>
        /// <param name="a"></param>
        /// <param name="aDir"></param>
        public void GetHash128ToDicByAssetBundleManifest(AssetBundleManifest a, Dictionary<string, Hash128> aDir)
        {
            string[] aName = a.GetAllAssetBundles();
            foreach (string name in aName)
            {
                aDir.Add(name, a.GetAssetBundleHash(name));
            }
        }

        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="needDownloadList">资源队列</param>
        public void DownLoadResources(List<string> needDownloadList)
        {
            string url = resServerUrl + PathUtils.GetRuntimePlatform();
            // 下载AB包
            foreach (var item in needDownloadList)
            {
                ResDownload.Instance.DownloadAssetBundle(url, item, OnResDonwloadSuccess);
            }
        }

        /// <summary>
        /// 资源下载成功回调
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="abName"></param>
        public void OnResDonwloadSuccess(string filePath, string abName)
        {
            Debug.Log(filePath + "<><><>" + abName);

            if (abName != PathUtils.GetRuntimePlatform())
            {
                // 计算进度条
                this.process = (float)cacheABPath.Count / (float)needDownloadList.Count;
                cacheABPath.Add(abName, filePath);
            }

            if (needDownloadList.Count == cacheABPath.Count && isDnoe != true) // 主清单文件下载完成
            {
                isDnoe = true;
                Debug.Log("所有文件下载完成！");
                try
                {
                    // 替换本地资源
                    foreach (var item in cacheABPath)
                    {
                        ReplaceLoaclRes(item.Value);
                    }

                    // 替换主Manifest文件
                    // 下载主Manifest文件
                    string url = resServerUrl + PathUtils.GetRuntimePlatform();
                    ResDownload.Instance.DownloadAssetBundle(url, PathUtils.GetRuntimePlatform(), (fp, n) =>
                    {
                        ReplaceLoaclRes(fp);

                        // 替换version.txt 文件
                        ResDownload.Instance.DownloadAssetBundle(url, "version.txt", (fp1, n1) =>
                        {
                            ReplaceLoaclRes(fp1);
                            Debug.Log("更新成功！");
                            ClearCacheFile();
                        });
                    });
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        /// <summary>
        /// 替换本地资源
        /// </summary>
        /// <param name="manifestPath"></param>
        /// <param name="path"></param>
        /// <param name="needDownloadList"></param>
        public void ReplaceLoaclRes(string cachePath)
        {
            string targetPath = ConvertPathByCache(cachePath);
            string targetDir = Path.GetDirectoryName(targetPath).Replace(@"\", "/");
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            //Debug.Log("cachePath:" + cachePath);
            //Debug.Log("targetPath:" + targetPath);
            File.Copy(cachePath, targetPath, true);
        }

        /// <summary>
        /// 通过缓存连接，获取到真实资源路径。
        /// </summary>
        /// <param name="cachePath"></param>
        /// <returns></returns>
        public string ConvertPathByCache(string cachePath)
        {
            return cachePath.Replace(PathUtils.GetRuntimePlatform() + "/cache", PathUtils.GetRuntimePlatform());
        }

        /// <summary>
        /// 清空下载缓存文件
        /// </summary>
        public void ClearCacheFile()
        {
            Directory.Delete(ResDownload.Instance.localDownDir, true);
        }

        /// <summary>
        /// 加载版本文件方法
        /// </summary>
        /// <param name="versionPath">版本文件路径</param>
        /// <param name="action">回调</param>
        /// <returns></returns>
        IEnumerator LoadVersion(string versionPath, Action<string, string> action)
        {
            WWW www = new WWW(versionPath);
            yield return www;

            if (www.error != null)
            {
                action(www.error, null);
            }
            else
            {
                action(null, www.text);
            }
            www.Dispose();
        }
    }
}

