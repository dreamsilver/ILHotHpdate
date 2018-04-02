using ILRuntime.Runtime.Enviorment;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using HotUpdateMessage;
using ILRuntime.CLR.Method;
using System.Threading;
using QCore.AssetMgr;
using XHFrame;

// public delegate void DelTest();

public class TestFrame : MonoBehaviour
{

    public static AppDomain appDomain;

    public static IMethod starts;

    private void Awake()
    {
        string DLL = "file:///" + Application.streamingAssetsPath + "/HotUpdateDLL.dll";
        string pdb = "file:///" + Application.streamingAssetsPath + "/HotUpdateDLL.pdb";
        StartCoroutine(InitHotUpdateConnect(DLL, pdb));
    }

    private void Start()
    {


    }

    bool isUpdate = false;
    IEnumerator InitHotUpdate()
    {
        yield return ABMgr.Instance.LoadManifest();
        yield return ABMgr.Instance.LoadAsset("hotscript", "HotUpdateDll.bytes", (m) =>
        {
            if (m is TextAsset)
            {
                appDomain = new AppDomain();
                byte[] bytes = (m as TextAsset).bytes;
                appDomain.LoadAssembly(new MemoryStream(bytes));
                OnHorFixLoaded();
                isUpdate = true;
            }
        });
    }


    private IEnumerator InitHotUpdateConnect(string dllUrl, string pdbUrl)
    {
        Debug.Log(string.Format("{0} {1}",dllUrl,pdbUrl));
        appDomain = new AppDomain();

        byte[] bytes = null;
        using (WWW www = new WWW(dllUrl))
        {
            while (!www.isDone)
                yield return null;
            if (www.error != null && !string.IsNullOrEmpty(www.error))
                Debug.LogError(www.error);
            bytes = www.bytes;
        }

        appDomain.LoadAssembly(new MemoryStream(bytes));
        // 可以注册ILRuntiem
        // 调用
        isUpdate = false;
        OnHorFixLoaded();
    }

    void OnHorFixLoaded()
    {

        #region 委托适配器

        appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((System.Action)act)();
            });
        });
        appDomain.DelegateManager.RegisterDelegateConvertor<ThreadStart>((act) =>
        {
            return new ThreadStart(() =>
            {
                ((System.Action)act)();
            });
        });
        appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();
        appDomain.DelegateManager.RegisterFunctionDelegate<IEnumerator>();
        appDomain.DelegateManager.RegisterMethodDelegate<Message>();
        appDomain.RegisterCrossBindingAdaptor(new IAsyncStateMachineClassInheritanceAdaptor());
        LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appDomain);

        #endregion

        appDomain.Invoke("HotUpdateDLL.TestOpenUI", "ThisUI", null, null);

        // StartCoroutine(Test());

        var obj = Resources.Load<GameObject>("Cube");
    }

    //public IEnumerator Test()
    //{
    //    //dicTestPath.Add("Canvas", "scene1/Canvas");
    //    //dicTestPath.Add("Login", "scene1/normal/Login");
    //    //dicTestPath.Add("Loading", "scene1/normal/Loading");
    //    //dicTestPath.Add("Home", "scene1/normal/Home");
    //    //dicTestPath.Add("PopOne", "scene1/popUP/PopOne")
    //    //yield return ABMgr.Instance.LoadManifest();
    //    //foreach (var item in collection)
    //    //{
    //    //    yield return ABMgr.Instance.LoadAsset("scene1/normal", "Login", m => { Debug.Log(m); });
    //    //}
    //}

    private void Update()
    {
        appDomain.Invoke("HotUpdateDLL.TestOpenUI", "Update", null, null);
    }

}
