using QCore.AssetMgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XHFrame;

[Serializable]
public class Test : MonoBehaviour
{
    private void Start()
    {
        for (int i = 0; i < 1; i++)
        {
            //InstanceManage.Instance.LoadCoroutineInstance("Cube", (m) =>
            //{
            //    GameObject go = m as GameObject;
            //    //go.transform.position = UnityEngine.Random.insideUnitSphere * 10;
            //});


            string path = "scene2/Canvas.prefab";

            //InstanceManage.Instance.LoadCoroutineInstance(path, (a) =>
            //{
            //    GameObject go = a as GameObject;
            //    // go.transform.position = UnityEngine.Random.insideUnitSphere * 10;

            //});

        }

        // StartCoroutine(enumerator());

    }

    public void OnClickButton()
    {
        Debug.Log("??");
        StartCoroutine(enumerators());
    }

    IEnumerator enumerators()
    {

        yield return ABMgr.Instance.LoadManifest();
        yield return ABMgr.Instance.LoadAsset("Scene2", "Canvas2.prefab", s =>
        {
            GameObject g = Instantiate(s as GameObject);
        });
    }

    IEnumerator enumerator()
    {
        yield return ABMgr.Instance.LoadManifest();
        yield return ABMgr.Instance.LoadAsset("Scene2", "Canvas.prefab", s =>
        {
            GameObject g = Instantiate(s as GameObject);
        });
    }
}
