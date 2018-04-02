using System;
using System.Collections;
using System.Collections.Generic;

namespace HotUpdateMessage
{
    public class Message : IEnumerable<KeyValuePair<string, object>>
    {
        private Dictionary<string, object> DicMessageDates = null;

        #region 字段属性

        /// <summary>
        /// 消息名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 发送者 一般为当前类
        /// </summary>
        public object Sender { get; private set; }

        /// <summary>
        /// 消息包含内容
        /// </summary>
        public object Content { get; set; }


        #endregion

        #region 实现索引器
        /// <summary>
        /// 消息索引器
        /// </summary>
        /// <param name="key">消息键</param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
                if (DicMessageDates == null || !DicMessageDates.ContainsKey(key))
                    return null;
                return DicMessageDates[key];
            }
            set
            {
                if (DicMessageDates == null)
                    DicMessageDates = new Dictionary<string, object>();
                if (DicMessageDates.ContainsKey(key))
                    DicMessageDates[key] = value;
                else
                    DicMessageDates.Add(key, value);
            }
        }
        #endregion

        #region 实现 IEnumerator 接口
        /// <summary>
        /// 时间IEnumerable接口
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            if (null == DicMessageDates)
                yield break;
            foreach (KeyValuePair<string, object> Kvp in DicMessageDates)
                yield return Kvp;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return DicMessageDates.GetEnumerator();
        }
        #endregion

        #region 消息构造函数

        /// <summary>
        /// 消息构造
        /// </summary>
        /// <param name="MessageName">消息名(类似定义List的变量名)</param>
        /// <param name="Sender">发送者 一般为当前类</param>
        public Message(string MessageName, object Sender)
        {
            this.Name = MessageName;
            this.Sender = Sender;
            this.Content = null;
        }

        /// <summary>
        /// 消息构造
        /// </summary>
        /// <param name="MessageName">消息名(类似定义List的变量名)</param>
        /// <param name="Sender">发送者 一般为当前类</param>
        /// <param name="Content">附加内容（必须为字典）</param>
        public Message(string MessageName, object Sender, object Content)
        {
            this.Name = MessageName;
            this.Sender = Sender;
            this.Content = Content;
        }

        /// <summary>
        /// 消息构造
        /// </summary>
        /// <param name="MessageName">消息名(类似定义List的变量名)</param>
        /// <param name="Sender">发送者 一般为当前类</param>
        /// <param name="Content">附加内容</param>
        /// <param name="_params">可变参数（必须为字典）</param>
        public Message(string MessageName, object Sender, object Content, params object[] _params)
        {
            this.Name = MessageName;
            this.Sender = Sender;
            this.Content = Content;
            for (int i = 0; i < _params.Length; i++)
            {
                if (_params[i].GetType() == typeof(Dictionary<string, object>))
                {
                    foreach (object dicParams in _params)
                    {
                        foreach (KeyValuePair<string, object> kvp in dicParams as Dictionary<string, object>)
                        {
                            // Debug.Log(kvp.Key);
                            this[kvp.Key] = kvp.Value;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// 消息构造
        /// </summary>
        /// <param name="message">消息体(封装好的消息内容)</param>
        public Message(Message message)
        {
            this.Name = message.Name;
            this.Content = message.Content;
            this.Sender = message.Sender;
            foreach (KeyValuePair<string, object> kvp in message.DicMessageDates)
            {
                DicMessageDates[kvp.Key] = kvp.Value;
            }
        }

        #endregion

        #region 添加&删除
        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="key">消息键</param>
        /// <param name="Value">消息内容</param>
        public void Add(string key, object Value)
        {
            this[key] = Value;
        }

        /// <summary>
        /// 移除消息信息
        /// </summary>
        /// <param name="key">需要移除的想消息键</param>
        public void Remove(string key)
        {
            if (DicMessageDates != null || DicMessageDates.ContainsKey(key))
            {
                DicMessageDates.Remove(key);
            }
        }

        #endregion

        #region 发送
        /// <summary>
        /// 发送消息
        /// </summary>
        public void Send(bool isCache = true)
        {
            MessageCenter.Instance.SendMessage(this, isCache);
        }
        #endregion
    }
}
