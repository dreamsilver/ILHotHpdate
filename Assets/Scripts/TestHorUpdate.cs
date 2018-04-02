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

// public delegate void DelTest();

public class TestHorUpdate : MonoBehaviour
{

    public Text text;
    public InputField input;
    public Button button;
    public Button Delete;

    public string path = "Cube";
    public static AppDomain appDomain;

    public static IMethod starts;

    private void Awake()
    {
        string DLL = "file:///" + Application.streamingAssetsPath + "/HotUpdateDLL.dll";
        string pdb = "file:///" + Application.streamingAssetsPath + "/HotUpdateDLL.pdb";

        text = transform.Find("Panel/Text").GetComponent<Text>();
        input = transform.Find("Panel/InputField").GetComponent<InputField>();
        button = transform.Find("Panel/Button").GetComponent<Button>();
        MessageCenter.Instance.AddListener("xiaoxi", (e) => { /*Debug.Log("空参消息");*/ });
        StartCoroutine(InitHotUpdateConnect(DLL, pdb));
        //StartCoroutine(InitHotUpdate());
    }

    private void Start()
    {
        //Message message = new Message("AAA", this);
        //message["testMessage"] = "messagesss";
        //message.Send();

        //MessageCenter.Send("123",this,"3333333333333");

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

        appDomain = new AppDomain();

        byte[] bytes = null;
        using (WWW www = new WWW(dllUrl))
        {
            while (!www.isDone)
                yield return null;
            if (www.error != null)
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
        starts = appDomain.GetType("HotUpdateDLL.TestHotUpdate").GetMethod("LoadAsss", 0);

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

        #endregion



        appDomain.Invoke("HotUpdateDLL.TestHotUpdate", "Test", null, text, button, Delete, input, this.gameObject, path);

        Message message = new Message("AAA", this);
        message["testMessage"] = "message";
        message.Send();

        Dictionary<string, object> SendCite = new Dictionary<string, object>();
        SendCite.Add("text", text);
        SendCite.Add("input", input);
        MessageCenter.Send("Init", this, SendCite);

        //Dictionary<string , object> dic = new Dictionary<string, object>();
        //dic.Add("aaa","ababab");
        //dic.Add("aaa1", "ababab1");
        //Debug.Log(dic.GetType());
        //// MessageCenter.Send("BBB",this,"abc", dic);
        //MessageCenter.Send("123", this, "3333333333333", dic);


        MessageCenter.Send("AddComponent", this, "UILogin");
        MessageCenter.Send("InitMessageSend", this);


    }


    private void OnApplicationQuit()
    {
        MessageCenter.Send("Quit", this);
    }


    private void Update()
    {
            appDomain.Invoke("HotUpdateDLL.TestHotUpdate", "Update", null);

    }

}
