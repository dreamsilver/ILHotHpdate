using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using HotUpdateMessage;
using System.Reflection;
using System.Threading;
using System.Collections;
using QCore.AssetMgr;
using XHFrame;

namespace HotUpdateDLL
{
    public class TestHotUpdate
    {
        static Text texts;
        static InputField inputs;
        static Button buttons;
        static UnityEngine.GameObject obj;
        private static string path;

        public static void Update()
        {
            ComponentFactory.Instance.Update();
        }

        public static void Test(Text text, Button button, Button delete, InputField inputField, GameObject objects, string paths)
        {
            ReferenceLadingManager.Instance.Init();

            Debug.Log("我被调用了:" + text.name + "<><><>" + button.name + "<><><>" + inputField.name);

            #region 调用消息测试
            //MessageCenter.Instance.AddListener("AAA", (M) =>
            //{
            //    foreach (var item in M)
            //    {
            //        Debug.Log(item + "<><><>" + 1);
            //    }
            //});

            //MessageCenter.Instance.AddListener("123", (M) =>
            //{
            //    Debug.Log(M.Content);

            //    foreach (var item in M)
            //    {
            //        Debug.Log(item + "<><><>" + 1);
            //    }
            //});
            #endregion


            MessageCenter.Instance.AddListener("Init", (M) =>
            {
                Dictionary<string, object> s = M.Content as Dictionary<string, object>;
                foreach (var item in s)
                {
                    if (item.Value is Text)
                    {
                        Text t = item.Value as Text;
                        t.text = "23333";
                        Debug.Log(t.text);
                    }
                }
            });

            texts = text;
            inputs = inputField;
            buttons = button;
            obj = objects;
            path = paths;
            button.onClick.AddListener(OnTestClick);
            delete.onClick.AddListener(OnDelete);

            // ReferenceLadingManager.Instance.CreateAScriptInstance("HotUpdateDLL", "UILogin");

            // 回發消息測試
            Message message = new Message("xiaoxi", "X");
            message.Send(false);
            Debug.Log("??");
            MessageCenter.Instance.AddListener("AddComponent", (m) =>
            {
                Debug.Log(m.Content);
                // Type t=Type.GetType("HotUpdateDLL."+m.Content.ToString());
                //object obj = Activator.CreateInstance(t);
                //FieldInfo[] fields = t.GetFields();
                //foreach (var item in fields)
                //{
                //    Debug.Log(item.Name);
                //}
                ////(obj as UILogin)?.Update();
                //Debug.Log(obj);
            });

        }


        public void TestMessage(Message message)
        {

        }

        public static void TestThread()
        {
            Debug.Log("this is Thread");

        }
        static int x;

        private static void OnTestClick()
        {

            texts.text = inputs.text;
            Debug.Log("我被点击了");
            UnityEngine.Object obj = Resources.Load(path);
            GameObject.Instantiate(obj);

            for (int i = 0; i < 1; i++)
            {

            }

            x++;
            // ReferenceLadingManager.Instance.UnityReferObjectInstance(obj as GameObject);

            Message m = new Message("OpenCoroutine", " ");
            m.Send();

            //GameObject.Instantiate(obj as GameObject);
        }



        public static object LoadAsss()
        {
            Debug.Log("调用携程");
            return new WaitForSeconds(2f);
        }

        private static void OnDelete()
        {
            Debug.Log("我点击了删除");
            GameObject go = null;
            foreach (var item in ReferenceLadingManager.Instance.dicScriptRefer.Keys)
            {
                Debug.Log("字典中有:" + item);
                go = item;
            }
            if (ReferenceLadingManager.Instance.dicScriptRefer.ContainsKey(go))
            {
                Debug.Log("我要删除物体:" + go.name);
                GameObject.Destroy(go);

                ReferenceLadingManager.Instance.dicScriptRefer.Remove(go);
            }
        }

    }
}
