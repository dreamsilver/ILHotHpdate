using QCore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
namespace QCore
{
    /// <summary>
    /// 日志信息
    /// </summary>
    public class Debugger : Core.Singleton<Debugger>
    {
        public Text text;

        private Debugger()
        {

        }

        public void Init()
        {
            Application.logMessageReceived += Application_logMessageReceived;
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            text.text += condition + "\n";
        }
    }
}