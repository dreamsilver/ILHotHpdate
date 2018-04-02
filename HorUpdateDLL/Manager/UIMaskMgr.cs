using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdateDLL
{
    public class UIMaskMgr : Singleton<UIMaskMgr>
    {
        private GameObject goCanvasRoot = null; // UI根节点对象


        private GameObject topPanel = null; // 顶部面板
        private GameObject maskPanel = null;// 遮罩面板

        private Camera uiCamera; // UI相机
        private float originalUICameralDepth; //UI相机原始的"层深"

        public UIMaskMgr()
        {
            // 得到UI根节点对象和脚本节点对象
            goCanvasRoot = GameObject.FindGameObjectWithTag(Define.GameObjectTag.SYS_TAG_CONVAS);
            // 得到顶点模板和遮罩面板
            topPanel = goCanvasRoot;
            maskPanel = UnityHelper.FindTheChildNode(goCanvasRoot, "_UIMaskPanel").gameObject;
            // 得到UI相机的层深
            uiCamera = GameObject.FindGameObjectWithTag("TagUICamera").GetComponent<Camera>();
            Debug.Log(uiCamera);
            if (uiCamera != null)
            {
                originalUICameralDepth = uiCamera.depth;
            }
            else
            {
                Debug.Log(GetType() + "/Start()/UI_Camera is Null!,Please Check! ");
            }

        }


        /// <summary>
        /// 设置遮罩状态
        /// </summary>
        /// <param name="goDisplayUIForm">需要显示的UI窗体</param>
        /// <param name="uIFormLucencyType">需要显示的遮罩透明度</param>
        public void SetMeskWindew(GameObject goDisplayUIForm, EnumUIFormLucencyType uIFormLucencyType = EnumUIFormLucencyType.Lucency)
        {
            topPanel.transform.SetAsLastSibling();
            switch (uIFormLucencyType)
            {
                case EnumUIFormLucencyType.Lucency:
                    maskPanel.SetActive(true);
                    Color color = new Color(Define.UIMaskConst.SYS_UIMASK_LUCENCY_COLOR_RGB, Define.UIMaskConst.SYS_UIMASK_LUCENCY_COLOR_RGB, Define.UIMaskConst.SYS_UIMASK_LUCENCY_COLOR_RGB, Define.UIMaskConst.SYS_UIMASK_LUCENCY_COLOR_RGB_A);
                    maskPanel.GetComponent<Image>().color = color;
                    break;
                case EnumUIFormLucencyType.Translucence:
                    maskPanel.SetActive(true);
                    Color color1 = new Color(Define.UIMaskConst.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB, Define.UIMaskConst.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB, Define.UIMaskConst.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB, Define.UIMaskConst.SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB_A);
                    maskPanel.GetComponent<Image>().color = color1;
                    break;
                case EnumUIFormLucencyType.ImPenetrable:
                    maskPanel.SetActive(true);
                    Color newColor3 = new Color(Define.UIMaskConst.SYS_UIMASK_IMPENETRABLE_COLOR_RGB, Define.UIMaskConst.SYS_UIMASK_IMPENETRABLE_COLOR_RGB, Define.UIMaskConst.SYS_UIMASK_IMPENETRABLE_COLOR_RGB, Define.UIMaskConst.SYS_UIMASK_IMPENETRABLE_COLOR_RGB_A);
                    maskPanel.GetComponent<Image>().color = newColor3;
                    break;
                case EnumUIFormLucencyType.Pentrate:
                    if (maskPanel.activeInHierarchy)
                    {
                        maskPanel.SetActive(false);
                    }
                    break;
                default:
                    break;
            }

            // 遮罩层下移
            maskPanel.transform.SetAsLastSibling();
            // 显示窗体下移
            goDisplayUIForm.transform.SetAsLastSibling();

            if (uiCamera != null)
            {
                uiCamera.depth = uiCamera.depth + 100;
            }
        }

        /// <summary>
        /// 取消遮罩状态
        /// </summary>
        public void CancelMaskWindew()
        {
            topPanel.transform.SetAsFirstSibling();
            if (maskPanel.activeInHierarchy)
            {
                maskPanel.SetActive(false);
            }

            if (uiCamera != null)
            {
                uiCamera.depth = originalUICameralDepth;
            }
        }
    }
}
