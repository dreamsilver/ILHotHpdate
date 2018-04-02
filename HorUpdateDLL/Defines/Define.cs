using System;
using System.Collections.Generic;
using UnityEngine;
using XHFrame;

namespace HotUpdateDLL
{

    /// <summary>
    /// 浮动UI显示的位置
    /// </summary>
    public enum EnumFloatingUIShowPosition
    {
        /// <summary>
        /// 上
        /// </summary>
        Top,
        /// <summary>
        /// 下
        /// </summary>
        Below,
        /// <summary>
        /// 左
        /// </summary>
        Left,
        /// <summary>
        /// 左上
        /// </summary>
        LeftTop,
        /// <summary>
        /// 左下
        /// </summary>
        LeftBelow,
        /// <summary>
        /// 右
        /// </summary>
        Right,
        /// <summary>
        /// 右上
        /// </summary>
        RightTop,
        /// <summary>
        /// 右下
        /// </summary>
        RightBelow,

    }

    /// <summary>
    /// UI的类型
    /// </summary>
    public enum EnumUIFormType
    {
        /// <summary>
        /// 普通窗体
        /// </summary>
        Normal,
        /// <summary>
        /// 固定窗体
        /// </summary>
        Fixed,
        /// <summary>
        /// 弹出窗体
        /// </summary>
        PopUp,
        /// <summary>
        /// 浮动窗体
        /// </summary>
        Floating
    }

    /// <summary>
    /// UI的显示方式
    /// </summary>
    public enum EnumUIFormShowMode
    {
        /// <summary>
        /// 普通
        /// </summary>
        Normal,
        /// <summary>
        /// 反向切换
        /// </summary>
        ReversChange,
        /// <summary>
        /// 隐藏其他
        /// </summary>
        HideOther,
        /// <summary>
        /// 浮动
        /// </summary>
        Floating
    }
    /// <summary>
    /// UI窗体遮罩层显示类型
    /// </summary>
    public enum EnumUIFormLucencyType
    {
        /// <summary>
        /// 完全透明 可以穿透
        /// </summary>
        Lucency,
        /// <summary>
        /// 半透明 不能穿透
        /// </summary>
        Translucence,
        /// <summary>
        /// 低透明 不能穿透
        /// </summary>
        ImPenetrable,
        /// <summary>
        /// 可以穿透
        /// </summary>
        Pentrate
    }

    /// <summary>
    /// UI对象
    /// </summary>
    public enum EnumUIFormObject
    {
        /// <summary>
        /// 空
        /// </summary>
        None = -1,
        /// <summary>
        /// 加载
        /// </summary>
        Loading = 0,
        /// <summary>
        /// 登录
        /// </summary>
        Login = 1,
        /// <summary>
        /// 主城
        /// </summary>
        Home = 2,
    }

    public enum EnumObjectState
    {
        /// <summary>
        /// 未知状态
        /// </summary>
        None,
        /// <summary>
        /// 初始化状态
        /// </summary>
        Initial,
        /// <summary>
        /// 过程状态(异步加载时)
        /// </summary>
        Loading,
        /// <summary>
        /// 已经打开
        /// </summary>
        Ready,
        /// <summary>
        /// 隐藏
        /// </summary>
        Disabled,
        /// <summary>
        /// 关闭
        /// </summary>
        Closing
    }


    /// <summary>
    /// 状态改变委托
    /// </summary>
    /// <param name="nowObject">当前的对象</param>
    /// <param name="pastState">过去的状态</param>
    /// <param name="newState">新的状态</param>
    public delegate void StateChangedEvent(object nowObject, EnumObjectState newState, EnumObjectState oldState);

    //public class UITool
    //{
    //    public class UI
    //    {
    //        public const string Loading = "scene1/Canvas";
    //        public const string Login = "scene1/Login";
    //        public const string Home = "scene1/Loading.prefab";
    //    }

    //    public static string GetUIPath(EnumUIFormObject uIType)
    //    {
    //        switch (uIType)
    //        {
    //            case EnumUIFormObject.None:
    //                return null;
    //            case EnumUIFormObject.Loading:
    //                return UI.Loading;
    //            case EnumUIFormObject.Login:
    //                return UI.Login;
    //            case EnumUIFormObject.Home:
    //                return UI.Home;
    //        }
    //        return null;
    //    }

    //}

    public class Define
    {
        public class UIPath
        {
            public const string SYS_PATH_CANVAS = "scene1/Canvas";
            public const string SYS_PATH_Loginl = "scene1/Fixed/Login";
            public const string SYS_PATH_Loading = "scene1/Fixed/Loading";
        }
        /// <summary>
        /// 标签常量
        /// </summary>
        public class GameObjectTag
        {
            public const string SYS_TAG_CONVAS = "TagConvas";
        }

        /// <summary>
        /// 节点常量
        /// </summary>
        public class GameObejctHoed
        {
            public const string SYS_NORMAL_NODE = "Normal";
            public const string SYS_FIXED_NODE = "Fixed";
            public const string SYS_POPUP_NODE = "PopUp";
        }
        /// <summary>
        /// 遮罩管理器中，透明度常量
        /// </summary>
        public class UIMaskConst
        {
            public const float SYS_UIMASK_LUCENCY_COLOR_RGB = 255 / 255F;
            public const float SYS_UIMASK_LUCENCY_COLOR_RGB_A = 0F / 255F;

            public const float SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB = 220 / 255F;
            public const float SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB_A = 50F / 255F;

            public const float SYS_UIMASK_IMPENETRABLE_COLOR_RGB = 50 / 255F;
            public const float SYS_UIMASK_IMPENETRABLE_COLOR_RGB_A = 200F / 255F;
        }
    }



    public class UIpathDefines
    {

    }
}
