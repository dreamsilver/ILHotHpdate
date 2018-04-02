using System;
using System.Collections.Generic;

namespace HotUpdateDLL
{
    internal class UIType
    {
        /// <summary>
        /// 是否清空"栈集合"信息
        /// </summary>
        public bool IsClearReverseChange = false;

        /// <summary>
        /// UI窗体类型(默认普通窗体)
        /// </summary>
        public EnumUIFormType _UIFormType = EnumUIFormType.Normal;

        /// <summary>
        /// UI窗体显示类型(默认普通) 
        /// </summary>
        public EnumUIFormShowMode _UIFormShowMode = EnumUIFormShowMode.Normal;

        /// <summary>
        /// UI窗体透明类型(默认完全透明,可以穿过)
        /// </summary>
        public EnumUIFormLucencyType _UIFormLucancyType = EnumUIFormLucencyType.Lucency;
        /// <summary>
        /// 浮动UI显示的位置(默认正下方)
        /// </summary>
        public EnumFloatingUIShowPosition _FloatingUIShowPosition = EnumFloatingUIShowPosition.Below;
    }
}
