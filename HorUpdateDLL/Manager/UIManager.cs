using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using XHFrame;

namespace HotUpdateDLL
{
    /// <summary>
    /// UI控制
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {

        #region MyRegion

        /// <summary>
        /// UI窗体预设路径(参数1:窗体预设名称 , 2:表示创意预设路径)
        /// </summary>
        public Dictionary<string, string> dicFormPath;

        /// <summary>
        /// 缓存所有的UI窗体
        /// </summary>
        private Dictionary<string, BaseUI> dicALLUIForm;

        /// <summary>
        /// 当前显示的UI窗体
        /// </summary>
        private Dictionary<string, BaseUI> dicCurrentShowUIForms;

        /// <summary>
        /// 保存已被加载出来的U对象
        /// </summary>
        public Dictionary<string, GameObject> dicBatchLoadUI;
        /// <summary>
        /// "栈集合"中保存的UI窗体
        /// </summary>
        private Stack<BaseUI> stackCurrentUIForm;
        /// <summary>
        /// UI根节点
        /// </summary>
        Transform traCanvasTransform = null;
        /// <summary>
        /// 全屏显示的节点
        /// </summary>
        Transform traNormal = null;
        /// <summary>
        /// 固定显示的节点
        /// </summary>
        Transform traFixed = null;
        /// <summary>
        /// 弹出节点
        /// </summary>
        Transform traPopUp = null;
        /// <summary>
        /// 悬浮UI节点
        /// </summary>
        Transform traFloatingUI = null;
        /// <summary>
        /// 获取canvas大小
        /// </summary>
        Vector2 canvasScope;
        #endregion

        #region 初始化脚本
        public override void Init()
        {
            base.Init();
            // 初始化集合
            dicFormPath = new Dictionary<string, string>();
            dicALLUIForm = new Dictionary<string, BaseUI>();
            dicCurrentShowUIForms = new Dictionary<string, BaseUI>();
            dicBatchLoadUI = new Dictionary<string, GameObject>();
            stackCurrentUIForm = new Stack<BaseUI>();

            // 初始化 UI的 Canvas
            InitRootCanvasLoading();

            traCanvasTransform = GameObject.FindGameObjectWithTag(Define.GameObjectTag.SYS_TAG_CONVAS).transform;
            if (traCanvasTransform == null)
            {
                Debug.LogError("UIManager初始化失败 , traCanvasTransform is null 请将Canvas的Tag 设置为 : TagConvas");
                return;
            }
            traNormal = traCanvasTransform.Find("Normal");
            traFixed = traCanvasTransform.Find("Fixed");
            traPopUp = traCanvasTransform.Find("PopUP");
            traFloatingUI = traCanvasTransform.Find("Floating");
            if (traNormal == null || traFixed == null || traPopUp == null || traFloatingUI == null)
            {
                Debug.LogError("UIManager初始化失败,没有找到:Normal,Fixed,PopUP 节点,请检查Canvar下是否包含这些节点 !!!!!   " + "Normal is >>" + traNormal.name + " , Fixed is >>" + traFixed.name + " , PopUP is >>" + traPopUp.name);
                return;
            }

            GameObject.DontDestroyOnLoad(traCanvasTransform);
            canvasScope = traCanvasTransform.GetComponent<RectTransform>().rect.size;
            this.canvasScope = new Vector2(canvasScope.x / 2, +canvasScope.y / 2);
            // 初始化UI窗体路径 (暂时不提供单个UI加载)
            // InitUIFormsPathsData();
        }

        #endregion

        #region 实例方法

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="UIName"></param>
        public void OpenUI(string UIName)
        {
            BaseUI baseUI = null;
            if (string.IsNullOrEmpty(UIName)) return;

            baseUI = LoadFormsToAllUIFormsCatch(UIName);
            if (baseUI == null) return;

            if (baseUI.SetUIType.IsClearReverseChange)
                ClearStackArraty(); // 清空栈集合中的数据
            switch (baseUI.SetUIType._UIFormShowMode)
            {
                case EnumUIFormShowMode.Normal:
                    LoadUIToCurrentCache(UIName);
                    break;
                case EnumUIFormShowMode.ReversChange:
                    PushUIFormToStack(UIName);
                    break;
                case EnumUIFormShowMode.HideOther:
                    EnterUIFormsAndHideOther(UIName);
                    break;
                case EnumUIFormShowMode.Floating:
                    FloatingUIFormsShow(UIName);
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="UIName"></param>
        public void CloseUI(string UIName)
        {
            BaseUI baseUI = null;
            if (string.IsNullOrEmpty(UIName)) return;

            dicALLUIForm.TryGetValue(UIName, out baseUI);
            if (baseUI == null) return;

            switch (baseUI.SetUIType._UIFormShowMode)
            {
                case EnumUIFormShowMode.Normal:
                    ExitUIForm(UIName);
                    break;
                case EnumUIFormShowMode.ReversChange:
                    PopUIForm(UIName);
                    break;
                case EnumUIFormShowMode.HideOther:
                    ExitUIFormsAndDisplayOther(UIName);
                    break;
                case EnumUIFormShowMode.Floating:
                    if (FloatingUIFormsObject != null)
                        FloatingUIFormsObject.Hiding();
                    break;
                default:
                    break;
            }
        }

        #endregion


        #region 私有方法

        #region 打开UI逻辑

        BaseUI FloatingUIFormsObject = null;
        /// <summary>
        /// 浮动窗口打开
        /// </summary>
        /// <param name="UIname"></param>
        /// <param name="vector"></param>
        void FloatingUIFormsShow(string UIname)
        {
            BaseUI baseUIForm;
            GameObject parentObj = EventSystem.current.currentSelectedGameObject;
            RectTransform parentRectTransform = parentObj.gameObject.GetComponent<RectTransform>();
            if (FloatingUIFormsObject != null)
            {
                FloatingUIFormsObject.Hiding();
                FloatingUIFormsObject = null;
            }

            dicALLUIForm.TryGetValue(UIname, out baseUIForm);
            if (baseUIForm != null)
            {
                FloatingUIFormsObject = baseUIForm;
                RectTransform rectTran = FloatingUIFormsObject.gameObject.GetComponent<RectTransform>();
                Vector2 scope = CreatePosition(rectTran, parentRectTransform, FloatingUIFormsObject);
                scope = new Vector2(Mathf.Abs(scope.x), Mathf.Abs(scope.y));
                if (canvasScope.x < scope.x || canvasScope.y < scope.y)
                {
                    switch (FloatingUIFormsObject.SetUIType._FloatingUIShowPosition)
                    {
                        case EnumFloatingUIShowPosition.Top:
                            CreatePosition(rectTran, parentRectTransform, EnumFloatingUIShowPosition.Below);
                            break;
                        case EnumFloatingUIShowPosition.Below:
                            CreatePosition(rectTran, parentRectTransform, EnumFloatingUIShowPosition.Top);
                            break;
                        case EnumFloatingUIShowPosition.Left:
                            CreatePosition(rectTran, parentRectTransform, EnumFloatingUIShowPosition.Right);
                            break;
                        case EnumFloatingUIShowPosition.LeftTop:
                            CreatePosition(rectTran, parentRectTransform, EnumFloatingUIShowPosition.RightBelow);
                            break;
                        case EnumFloatingUIShowPosition.LeftBelow:
                            CreatePosition(rectTran, parentRectTransform, EnumFloatingUIShowPosition.RightTop);
                            break;
                        case EnumFloatingUIShowPosition.Right:
                            CreatePosition(rectTran, parentRectTransform, EnumFloatingUIShowPosition.Left);
                            break;
                        case EnumFloatingUIShowPosition.RightTop:
                            CreatePosition(rectTran, parentRectTransform, EnumFloatingUIShowPosition.LeftBelow);
                            break;
                        case EnumFloatingUIShowPosition.RightBelow:
                            CreatePosition(rectTran, parentRectTransform, EnumFloatingUIShowPosition.LeftTop);
                            break;
                        default:
                            break;

                    }
                }
                FloatingUIFormsObject.Display();
            }
        }

        /// <summary>
        /// 浮动窗口显示的位置
        /// </summary>
        /// <param name="floattingWindew"></param>
        /// <param name="parentObject"></param>
        /// <param name="floattingWindewBaseUI"></param>
        Vector2 CreatePosition(RectTransform floattingWindew, RectTransform parentObject, BaseUI floattingWindewBaseUI)
        {
            EnumFloatingUIShowPosition floatingUIShowPosition = floattingWindewBaseUI.SetUIType._FloatingUIShowPosition;

            return CreatePosition(floattingWindew, parentObject, floatingUIShowPosition);
        }
        /// <summary>
        /// 浮动窗口显示的位置
        /// </summary>
        /// <param name="floattingWindew"></param>
        /// <param name="parentObject"></param>
        /// <param name="floattingWindewBaseUI"></param>
        Vector2 CreatePosition(RectTransform floattingWindew, RectTransform parentObject, EnumFloatingUIShowPosition floatingUIShowPosition)
        {

            Vector2 scope = Vector2.zero;
            switch (floatingUIShowPosition)
            {
                case EnumFloatingUIShowPosition.Top:
                    //正上
                    floattingWindew.pivot = new Vector2(0.5f, 0f); ;
                    floattingWindew.localPosition = parentObject.localPosition + new Vector3(0, parentObject.rect.size.y / 2, 0);
                    scope.x = floattingWindew.localPosition.x;
                    scope.y = -floattingWindew.localPosition.y + -floattingWindew.rect.size.y;
                    break;
                case EnumFloatingUIShowPosition.Below:
                    // 正下
                    floattingWindew.pivot = new Vector2(0.5f, 1f); ;
                    floattingWindew.localPosition = parentObject.localPosition - new Vector3(0, parentObject.rect.size.y / 2, 0);
                    scope.x = floattingWindew.localPosition.x;
                    scope.y = -floattingWindew.localPosition.y + floattingWindew.rect.size.y;
                    break;
                case EnumFloatingUIShowPosition.Left:
                    //左
                    floattingWindew.pivot = new Vector2(1, 0);
                    floattingWindew.localPosition = parentObject.localPosition - new Vector3(parentObject.rect.size.x / 2, parentObject.rect.size.y / 2, 0);
                    scope.x = -floattingWindew.localPosition.x + floattingWindew.rect.size.x;
                    scope.y = floattingWindew.localPosition.y + floattingWindew.rect.size.y;
                    break;
                case EnumFloatingUIShowPosition.LeftTop:
                    //左上
                    floattingWindew.pivot = new Vector2(1, 0);
                    floattingWindew.localPosition = parentObject.localPosition - new Vector3(parentObject.rect.size.x / 2, -parentObject.rect.size.y / 2, 0);
                    scope.x = -floattingWindew.localPosition.x + floattingWindew.rect.size.x;
                    scope.y = floattingWindew.localPosition.y + floattingWindew.rect.size.y;
                    break;
                case EnumFloatingUIShowPosition.LeftBelow:
                    // 左下
                    floattingWindew.pivot = Vector2.one;
                    floattingWindew.localPosition = parentObject.localPosition - new Vector3(parentObject.rect.size.x / 2, parentObject.rect.size.y / 2, 0);
                    scope.x = -floattingWindew.localPosition.x + floattingWindew.rect.size.x;
                    scope.y = -floattingWindew.localPosition.y + floattingWindew.rect.size.y;
                    break;
                case EnumFloatingUIShowPosition.Right:
                    //右
                    floattingWindew.pivot = Vector2.zero;
                    floattingWindew.localPosition = parentObject.localPosition + new Vector3(parentObject.rect.size.x / 2, -parentObject.rect.size.y / 2, 0);
                    scope.x = floattingWindew.localPosition.x + floattingWindew.rect.size.x;
                    scope.y = floattingWindew.localPosition.y + floattingWindew.rect.size.y;
                    break;
                case EnumFloatingUIShowPosition.RightTop:
                    //右上
                    floattingWindew.pivot = Vector2.zero;
                    floattingWindew.localPosition = parentObject.localPosition + new Vector3(parentObject.rect.size.x / 2, parentObject.rect.size.y / 2, 0);
                    scope.x = floattingWindew.localPosition.x + floattingWindew.rect.size.x;
                    scope.y = floattingWindew.localPosition.y + floattingWindew.rect.size.y;
                    break;
                case EnumFloatingUIShowPosition.RightBelow:
                    //右下
                    floattingWindew.pivot = new Vector2(0, 1);
                    floattingWindew.localPosition = parentObject.localPosition - new Vector3(-parentObject.rect.size.x / 2, parentObject.rect.size.y / 2, 0);
                    scope.x = floattingWindew.localPosition.x + floattingWindew.rect.size.x;
                    scope.y = -floattingWindew.localPosition.y + floattingWindew.rect.size.y;
                    break;
                default:
                    break;
            }
            return scope;
        }



        /// <summary>
        /// "隐藏窗体"关闭窗体,且显示其他窗体
        /// </summary>
        /// <param name="UIname"></param>
        private void EnterUIFormsAndHideOther(string uIName)
        {
            BaseUI baseUIForm; // UI窗体基类
            BaseUI baseUIFormFromALL; // 从集合中获取的UI窗体

            if (string.IsNullOrEmpty(uIName)) return;

            // 将正在"正在显示的UI"和栈集合中的UI全部设置为隐藏
            dicCurrentShowUIForms.TryGetValue(uIName, out baseUIForm);
            if (baseUIForm != null) return;
            foreach (var item in dicCurrentShowUIForms.Values)
            {
                item.Hiding();
            }
            foreach (var item in stackCurrentUIForm)
            {
                item.Hiding();
            }

            // 将当前窗体加入到"正在显示的UI"集合中, 并且做显示处理
            dicALLUIForm.TryGetValue(uIName, out baseUIFormFromALL);
            if (baseUIFormFromALL != null)
            {
                dicCurrentShowUIForms.Add(uIName, baseUIFormFromALL);
                baseUIFormFromALL.Display();
            }
        }

        /// <summary>
        /// 将UI加载到"当前窗体"的集合中
        /// </summary>
        /// <param name="UIname"></param>
        private void PushUIFormToStack(string uIName)
        {
            BaseUI baseUI = null;
            // 判断栈集合中 是否包含其他UI 否则做冻结处理
            if (stackCurrentUIForm.Count > 0)
            {
                BaseUI topBaseUI = stackCurrentUIForm.Peek();
                topBaseUI.Freeze(); // 栈顶元素冻结处理
            }

            dicALLUIForm.TryGetValue(uIName, out baseUI);
            if (baseUI != null)
            {
                baseUI.Display(); //将当前窗体设置为显示状态
                stackCurrentUIForm.Push(baseUI); // 将当前窗体入栈操作
            }
            else
            {
                Debug.LogError("baseUIForm==null,Please Check, 参数 uiFormName=" + uIName);
            }
        }

        /// <summary>
        /// 将UI加载到"当前窗体"的集合中
        /// </summary>
        /// <param name="UIname"></param>
        private void LoadUIToCurrentCache(string uIName)
        {
            BaseUI baseUI = null;
            BaseUI baseUIFromAllCache = null;

            // 查看当前显示得到UI窗体中是否包含这个UI窗体,如果包含 直接返回
            dicCurrentShowUIForms.TryGetValue(uIName, out baseUI);
            if (baseUI != null)
                return;
            // 将当前窗体添加到正在显示的集合中
            dicALLUIForm.TryGetValue(uIName, out baseUIFromAllCache);
            if (baseUIFromAllCache != null)
            {
                dicCurrentShowUIForms.Add(uIName, baseUIFromAllCache);
                baseUIFromAllCache.Display();
            }

        }
        #endregion

        #region 关闭UI逻辑

        /// <summary>
        /// 退出指定UI并显示其他已隐藏UI
        /// </summary>
        /// <param name="uIName"></param>
        private void ExitUIFormsAndDisplayOther(string uIName)
        {
            BaseUI baseUI = null;
            if (string.IsNullOrEmpty(uIName)) return;

            dicCurrentShowUIForms.TryGetValue(uIName, out baseUI);
            if (baseUI == null) return;

            // 将当前UI状态设置为隐藏,并从显示集合中 移除当前窗体
            baseUI.Hiding();
            dicCurrentShowUIForms.Remove(uIName);

            // 将其他窗体重新显示出来
            foreach (var item in dicCurrentShowUIForms.Values)
            {
                item.Redisplay();
            }
            foreach (var item in stackCurrentUIForm)
            {
                item.Redisplay();
            }
        }

        /// <summary>
        /// (反向切换UI) 窗体出站逻辑
        /// </summary>
        private void PopUIForm(string uIName)
        {
            if (stackCurrentUIForm.Count >= 2)
            {
                BaseUI topUIForm = stackCurrentUIForm.Pop();
                topUIForm.Hiding();

                BaseUI enxtUIform = stackCurrentUIForm.Peek();
                enxtUIform.Redisplay();
            }
            else if (stackCurrentUIForm.Count == 1)
            {
                BaseUI topUIForm = stackCurrentUIForm.Pop();
                topUIForm.Hiding();
            }
        }

        /// <summary>
        /// 退出指定UI
        /// </summary>
        /// <param name="UIname"></param>
        private void ExitUIForm(string uIName)
        {
            BaseUI baseUI = null;

            // 如果"当前显示的UI"不包含这个UI ,就直接返回
            dicCurrentShowUIForms.TryGetValue(uIName, out baseUI);
            if (baseUI == null) return;
            // 隐藏UI 并从"当前显示的UI"集合中移除UI
            baseUI.Hiding();
            dicCurrentShowUIForms.Remove(uIName);
        }
        #endregion

        /// <summary>
        /// 清空栈集合中的数据
        /// </summary>
        private bool ClearStackArraty()
        {
            if (stackCurrentUIForm != null && stackCurrentUIForm.Count > 0)
            {
                stackCurrentUIForm.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 初始化加载根窗体Convas预制
        /// </summary>
        private void InitRootCanvasLoading()
        {
            var canvas = Resources.Load("Canvas");
            GameObject.Instantiate(canvas);
        }


        /// <summary>
        /// 初始化UI窗口路径
        /// </summary>
        private void InitUIFormsPathsData()
        {
            return;
        }

        /// <summary>
        /// 根据UI窗体的名称,加载到"所有UI窗体"缓存集合中
        /// 功能:检查"所有UI窗体"中是否已经加载过,如果没有就进行加载
        /// </summary>
        /// <param name="UIName"></param>
        private BaseUI LoadFormsToAllUIFormsCatch(string uIName)
        {
            BaseUI baseUI = null;
            dicALLUIForm.TryGetValue(uIName, out baseUI);
            if (baseUI == null)
                baseUI = LoadBaseUI(uIName);
            return baseUI;
        }
        /// <summary>
        /// 加载制定名称的UI窗体
        /// 功能 : 
        ///         1:根据UI的名称加载预制体,
        ///         2:根据不同UI中的UI脚本上的UI信息,加载到根窗体下不同的UI节点上
        ///         3:隐藏刚创建的UI克隆体
        ///         4:把加载出来的UI克隆体加载到"所有UI窗体"的集合中
        /// </summary>
        /// <param name="UIname"></param>
        /// <returns></returns>
        private BaseUI LoadBaseUI(string uIName)
        {
            GameObject goCloneUIPrefad = null;
            BaseComponent baseComponent = null;
            BaseUI baseUI = null;

            dicBatchLoadUI.TryGetValue(uIName, out goCloneUIPrefad);
            if (goCloneUIPrefad == null)
                Debug.LogError("UIManager::LoadBaseUI()>>goCloneUIPrefad 是空的,请检查需要打开的UI名是否与配置文件中相同!!" + goCloneUIPrefad);
            GameObject initgo = GameObject.Instantiate(goCloneUIPrefad);
            if (goCloneUIPrefad != null && traCanvasTransform != null && initgo != null)
            {
                ReferenceLadingManager.Instance.dicScriptRefer.TryGetValue(initgo, out baseComponent);

                baseUI = baseComponent as BaseUI;
                if (baseUI == null)
                {
                    Debug.LogError("baseUi ==null! ,请先确认窗体预设对象上是否加载了baseUI的子类脚本！ 参数 uiName=" + uIName);
                    return null;
                }
                switch (baseUI.SetUIType._UIFormType)
                {
                    case EnumUIFormType.Normal:
                        initgo.transform.SetParent(traNormal, false);
                        break;
                    case EnumUIFormType.Fixed:
                        initgo.transform.SetParent(traFixed, false);
                        break;
                    case EnumUIFormType.PopUp:
                        initgo.transform.SetParent(traPopUp, false);
                        break;
                    case EnumUIFormType.Floating:
                        initgo.transform.SetParent(traFloatingUI, false);
                        break;
                }
                baseUI.ThisUIName = uIName;
                initgo.SetActive(false); // 隐藏UI
                dicALLUIForm.Add(uIName, baseUI);
                return baseUI;
            }
            else
            {
                Debug.LogError("traCanvasTransfrom==null Or goCloneUIPrefad==null!! ,Plese Check!, 参数uiFormName=" + uIName);
            }
            return null;

        }
        #endregion

    }
}
