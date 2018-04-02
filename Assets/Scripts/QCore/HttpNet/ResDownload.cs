 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using QCore.AssetMgr;
using System.IO;
using UnityEngine;
using QCore.Core;

namespace QCore.HttpNet
{
    /// <summary>
    /// 资源下载
    /// </summary>
    public class ResDownload : Core.Singleton<ResDownload>
    {
        public string localDownDir = PathUtils.GetPersistentDataPath() + "/cache/";

        public async void DownloadAssetBundle(string url,string abName,Action<string,string> action)
        {
            string remotePath = url + "/" + abName;
            // 本地更新资源缓存目录

            // 用于检查本地文件夹是否存在
            string temoLocalPath = localDownDir;
            if (abName.Contains("/"))
            {
                int index = abName.LastIndexOf("/");
                string temp = abName.Substring(0, index);
                temoLocalPath += temp;
            }

            Debug.Log("remotePath：" + remotePath);
            Debug.Log("localPath：" + localDownDir);
            Debug.Log("temoLocalPath：" + temoLocalPath);
            if (!Directory.Exists(temoLocalPath))
            {
                Directory.CreateDirectory(temoLocalPath);
            }

            // 本地缓存目录中ab包
            string filePath = localDownDir + abName;

            // 
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                // 获取文件现在的长度
                long fileLength = fs.Length;

                // 获取需要下载文件的总长度
                long totalLength = await GetResLengthAsync(remotePath);

                // 判断是否没有下载完成
                if (fileLength < totalLength)
                {
                    Debug.Log("开始下载：" + abName);
                    // 文件流指针便宜，断点续传核心。
                    fs.Seek(fileLength, SeekOrigin.Begin);

                    HttpWebRequest req = WebRequest.Create(remotePath) as HttpWebRequest;
                    // 设置远程文件流开始位置，断点续传核心。
                    req.AddRange(fileLength);

                    // 响应流
                    using (Stream stream = req.GetResponse().GetResponseStream())
                    {
                        byte[] buffer = new byte[1024];

                        int len = stream.Read(buffer, 0, buffer.Length);

                        while (len > 0)
                        {
                            // 将数据写入到文件
                            fs.Write(buffer, 0, len);
                            fileLength += len;

                            // 计算进度
                            // UnityEngine.Debug.Log(abName + ":" + (float)fileLength / (float)totalLength);

                            len = stream.Read(buffer, 0, buffer.Length);
                        }
                    }  
                }
                Debug.Log("下载完成：" + abName);
            }
            action(filePath, abName);
        }

        /// <summary>
        /// 获取资源文件的长度（异步）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<long> GetResLengthAsync(string url)
        {
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Method = "HEAD";

            HttpWebResponse resp =  await req.GetResponseAsync() as HttpWebResponse;
            return resp.ContentLength;
        }
    }
}