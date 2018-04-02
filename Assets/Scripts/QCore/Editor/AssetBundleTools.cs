using QCore.AssetMgr;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// AB包工具
/// </summary>
public class AssetBundleTools : ScriptableObject
{
    /// <summary>
    /// 设置assetbundle标签
    /// </summary>
    /// <param name="isRemove"></param>
    [MenuItem("Asset Bundle/标签/标记所有")]
    public static void SetAllLabels()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();

        // 需要打包的资源目录
        DirectoryInfo resDir = new DirectoryInfo(PathUtils.resPath);

        // 资源目录不存在
        if (!resDir.Exists)
            return;

        // 获取资源目录下的所有文件、文件夹
        DirectoryInfo[] sceneDir = resDir.GetDirectories();
        foreach (var item in sceneDir)
        {
            var sceneName = item.Name;

            // 每一个场景文件夹
            SetAllLabels(item, sceneName);
        }

    }

    [MenuItem("Asset Bundle/标签/移除所有标记")]
    public static void RemoveAllLables()
    {
        foreach (var item in AssetDatabase.GetAllAssetBundleNames())
        {
            AssetDatabase.RemoveAssetBundleName(item, true);
        } 
    }

    /// <summary>
    /// windows打包
    /// </summary>
    [MenuItem("Asset Bundle/Build/Windows")]
    public static void BuildWindows()
    {
        Build(PathUtils.windowTargetPath, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Asset Bundle/Build/Windows 到服务器")]
    public static void BuildWindowsToServer()
    {
        Build(PathUtils.ResServerPath + "/Windows", BuildTarget.StandaloneWindows);
        using (var sw = File.CreateText(PathUtils.ResServerPath+"/Windows/version.txt"))
        {
            sw.WriteLine("1.0");
        }
    }

    [MenuItem("Asset Bundle/Build/Android")]
    public static void BuildAndroid()
    {
        Build(PathUtils.androidTargetPath, BuildTarget.Android);
    }

    [MenuItem("Asset Bundle/更新脚本")]
    public static void UpdateHotUpScript()
    {
        string filePath = Application.streamingAssetsPath + "/HotUpdateDll.dll";
        if (File.Exists(filePath))
        {
            string movePath = PathUtils.resPath + "/HotScript";
            if (!Directory.Exists(movePath))
                Directory.CreateDirectory(movePath);

            File.Copy(filePath, movePath + "/HotUpdateDll.bytes",true);

            Debug.Log("更新打包脚本成功！");
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Asset Bundle/Build/创建 版本文件")]
    public static void CreateVersion()
    {
        if(Directory.Exists(PathUtils.windowTargetPath))
            using (var sw = File.CreateText(PathUtils.windowTargetPath + "/version.txt"))
            {
                sw.WriteLine("1.0");
            }
        if (Directory.Exists(PathUtils.androidTargetPath))
            using (var sw = File.CreateText(PathUtils.androidTargetPath + "/version.txt"))
            {
                sw.WriteLine("1.0");
            }
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 删除所有打包
    /// </summary>
    [MenuItem("Asset Bundle/Build/移除所有AB包")]
    public static void DeleteAll()
    {
        File.Delete(PathUtils.windowTargetPath + ".meta");
        File.Delete(PathUtils.androidTargetPath + ".meta");
        Directory.Delete(PathUtils.windowTargetPath,true);
        Directory.Delete(PathUtils.androidTargetPath, true);
        AssetDatabase.Refresh();
    }

    [MenuItem("Asset Bundle/打开 服务器资源目录")]
    public static void OpenServerResDir()
    {
        System.Diagnostics.Process.Start(PathUtils.ResServerPath);
    }
    [MenuItem("Asset Bundle/打开 本地更新目录")]
    public static void OpenPersistentDataDir()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }


    /// <summary>
    /// 打包
    /// </summary>
    /// <param name="targetPath"></param>
    /// <param name="buildTarget"></param>
    public static void Build(string targetPath, BuildTarget buildTarget)
    {
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        BuildPipeline.BuildAssetBundles(targetPath, BuildAssetBundleOptions.None, buildTarget);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 遍历指定文件夹中的文件
    /// </summary>
    /// <param name="fileSystemInfo"></param>
    /// <param name="sceneName"></param>
    public static void SetAllLabels(FileSystemInfo fileSystemInfo,string sceneName)
    {
        // 目录
        var dir = fileSystemInfo as DirectoryInfo;

        // 拿到当前目录下的文件
        var fsis = dir.GetFileSystemInfos();

        foreach (var item in fsis)
        {
            // 如果是文件，就直接设置assetbundle标签。
            var fileInfo = item as FileInfo;
            if(fileInfo != null)
            {
                SetLabel(fileInfo, sceneName);
            }
            else
            {
                SetAllLabels(item, sceneName);
            }
        }
    }
     
    /// <summary>
    /// 设置文件的assetbundle标签
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="sceneName"></param>
    public static void SetLabel(FileInfo fileInfo,string sceneName)
    {
        // 把window \ 路径 转换为unity路径 /
        var path = fileInfo.FullName.Replace(@"\","/");
        if (fileInfo.Extension == ".meta")
            return;

        // 截取出场景后面的路径
        var startIndex = path.LastIndexOf(sceneName) + sceneName.Length + 1;

        path = path.Substring(startIndex);

        var name = sceneName;

        // 如果包含 / 表示不在 场景目录下
        if (path.Contains("/"))
        {
            var twoDir = path.Split('/')[0];
            name = sceneName + "/" + twoDir;
        }

        // 设置assetbundle标签
        AssetImporter assetImporter = AssetImporter.GetAtPath("Assets/Res/" + sceneName + "/" + path);
        assetImporter.assetBundleName = name;
        if (fileInfo.Extension != ".unity")
        {
            assetImporter.assetBundleVariant = "assetbundle";
        }
        else
        {
            assetImporter.assetBundleVariant = "unity3d";
        }
    }
}
