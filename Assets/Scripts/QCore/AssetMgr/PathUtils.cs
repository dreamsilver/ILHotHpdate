#define Test
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
namespace QCore.AssetMgr
{
    /// <summary>
    /// 路径类
    /// </summary>
    public class PathUtils
    {
#if UNITY_EDITOR
        /// <summary>
        /// 打包存放的路径
        /// </summary>
        public readonly static string buildPath = Application.streamingAssetsPath + "/Res";

        /// <summary>
        /// 各个平台包，存放路径。
        /// </summary>
        public readonly static string windowTargetPath = buildPath + "/Windows";
        public readonly static string androidTargetPath = buildPath + "/Android";

        public readonly static string ResServerPath = @"C:\Program Files\Unity2017\nginx\html";

        /// <summary>
        /// 需要设置标签的路径
        /// </summary>
        public readonly static string resPath = Application.dataPath + "/Res";
#endif
        /// <summary>
        /// 获取运行时平台中AB包的路径
        /// </summary>
        /// <returns></returns>
        public static string GetStreamingAssetsPath()
        {
            return Application.streamingAssetsPath + "/Res/" + GetRuntimePlatform();
        }

        public static string GetPersistentDataAssetsPath()
        {
            string path = GetPersistentDataPath();
            if (GetRuntimePlatform() == "Android")
            {
                return "jar:file://" + path;
            }
            else
            {
                return "file:///" + path;
            }
        }

        public static string GetPersistentDataPath()
        {
            return Application.persistentDataPath + "/Res/" + GetRuntimePlatform(); 
        }

        /// <summary>
        /// 获取AB包路径
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public static string GetAssetsBundlePath(string abName)
        {
            return GetFilePath(abName);
        }

        /// <summary>
        /// 获取对应平台下数据文件的路径
        /// </summary>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        public static string GetFilePath(string dataPath)
        {
            string path = GetPersistentDataPath() + "/" + dataPath;
            if (!File.Exists(path))
                path = GetStreamingAssetsPath() + "/" + dataPath;
            else
                path = GetPersistentDataAssetsPath() + "/" + dataPath; 
            return path;
        }

        /// <summary>
        /// 获取运行时平台
        /// </summary>
        /// <returns></returns>
        public static string GetRuntimePlatform()
        {
            string platform = "";
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                platform = "Windows";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                platform = "Android";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                platform = "IOS";
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                platform = "OSX";
            }
            return platform;
        }
    }
}