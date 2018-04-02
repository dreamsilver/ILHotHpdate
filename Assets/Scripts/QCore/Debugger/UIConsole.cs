using QCore.AssetMgr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConsole : MonoBehaviour
{
    public bool isOpen = true;

    public GameObject g1;
    public GameObject g2;

    public Text btnText;

    public InputField abName;
    public InputField assetName;
   
    public void OpenConsole()
    {
        if (isOpen)
        {
            g1.SetActive(false);
            g2.SetActive(false);
            btnText.text = "打开";
        }
        else
        {
            g1.SetActive(true);
            g2.SetActive(true);
            btnText.text = "关闭";
        }
        isOpen = !isOpen;
    }

    public void LoadAsset()
    {
        Debug.Log("LoadAsset()"+ abName.text+" | " + assetName.text);
        StartCoroutine(ABMgr.Instance.LoadAsset(abName.text, assetName.text, o =>
        {
            Instantiate(o);
        }));
    }
}
