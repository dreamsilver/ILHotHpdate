using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdateDLL
{
    public abstract class BaseUI : BaseComponent
    {
        #region UI窗体属性&字段
        /// <summary>
        /// UI的当前窗体属性
        /// </summary>
        private UIType _uiType = new UIType();

        /// <summary>
        /// 设置当前UI的窗体属性
        /// </summary>
        internal UIType SetUIType
        {
            get
            {
                return _uiType;
            }
            set
            {
                _uiType = value;
            }
        }

        public string ThisUIName { get; set; }

        #endregion

        #region UI窗体方法

        /// <summary>
        /// 显示页面
        /// </summary>
        public virtual void Display()
        {
            this.gameObject.SetActive(true);
            if (SetUIType._UIFormType == EnumUIFormType.PopUp)
            {
                UIMaskMgr.Instance.SetMeskWindew(this.gameObject, SetUIType._UIFormLucancyType);
            }
        }

        /// <summary>
        /// 界面隐藏 (不在栈集合中)
        /// </summary>
        public virtual void Hiding()
        {
            this.gameObject.SetActive(false);
            if (SetUIType._UIFormType == EnumUIFormType.PopUp)
            {
                UIMaskMgr.Instance.CancelMaskWindew();
            }
        }

        /// <summary>
        /// 页面重新显示
        /// </summary>
        public virtual void Redisplay()
        {
            this.gameObject.SetActive(true);
            if (SetUIType._UIFormType == EnumUIFormType.PopUp)
            {
                UIMaskMgr.Instance.SetMeskWindew(this.gameObject, SetUIType._UIFormLucancyType);
            }
        }

        /// <summary>
        /// 页面冻结(还在"栈"集合中)
        /// </summary>
        public virtual void Freeze()
        {
            gameObject.SetActive(true);
        }


        #region 常用方法封装

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="UIName"></param>
        public void OpenUI(string UIName)
        {
            UIManager.Instance.OpenUI(UIName);
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="UIname"></param>
        public void CloseUI(string UIname)
        {
            UIManager.Instance.CloseUI(UIname);
        }
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="UIname"></param>
        public void CloseUI()
        {
            UIManager.Instance.CloseUI(ThisUIName);
        }
        /// <summary>
        /// Button点击事件
        /// </summary>
        /// <param name="ButtonName"></param>
        /// <param name="OnClick"></param>
        public void ButtonCilck(string ButtonName, UnityEngine.Events.UnityAction OnClick)
        {
            Button b = UnityHelper.GetTheChildNodeComponetScripts<Button>(gameObject, ButtonName);
            b.onClick.AddListener(OnClick);
        }

        public void ButtonCilck(Button buttonObj, UnityEngine.Events.UnityAction OnClick)
        {
            buttonObj.onClick.AddListener(OnClick);
        }

        #endregion

        #endregion

    }
}
