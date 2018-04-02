using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdateMessage
{

    public class MessageCenter : Singleton<MessageCenter>
    {
        private Dictionary<string, List<Action<Message>>> DicMessageEvents = null;

        private Dictionary<string, List<Message>> DicCacheMessageEvents = null;

        /// <summary>
        /// 初始化内容
        /// </summary>
        public override void Init()
        {
            base.Init();
            DicMessageEvents = new Dictionary<string, List<Action<Message>>>();
            DicCacheMessageEvents = new Dictionary<string, List<Message>>();
        }

        #region   添加监听 & 删除监听


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="MessageType">消息名</param>
        /// <param name="Sender">发送者（一般为当前类）</param>
        /// <param name="Content">发送内容</param>
        /// <param name="_params">可变参数（必须为字典）</param>
        public static void Send(MessageType MessageType, object Sender, object Content, bool isCache = true, params object[] _params)
        {
            Send(MessageType.ToString(), Sender, Content, isCache, _params);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="MessageName">消息名</param>
        /// <param name="Sender">发送者（一般为当前类）</param>
        /// <param name="Content">发送内容</param>
        /// <param name="_params">可变参数（必须为字典）</param>
        public static void Send(string MessageName, object Sender, object Content, bool isCache = true, params object[] _params)
        {

            // Debug.Log($"MessageCenter::Send() MessageName:{MessageName}");

            if (Instance == null)
                return;
            Message message = new Message(MessageName, Sender, Content, isCache, _params);
            Instance.SendMessage(message);
        }

        /// <summary>
        /// 空参消息
        /// </summary>
        /// <param name="MessageName"></param>
        /// <param name="Sender"></param>
        public static void Send(string MessageName, object Sender, bool isCache = true)
        {
            if (Instance == null)
                return;
            Message message = new Message(MessageName, Sender, null);
            Instance.SendMessage(message, isCache);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="messageType">添加监听事件类型(通过事件类型添加事件监听的对象名)</param>
        /// <param name="messageEvent"></param>
        public void AddListener(MessageType messageType, Action<Message> messageEvent)
        {

            AddListener(messageType.ToString(), messageEvent);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="MessageName">添加监听事件名(通过字符串添加监听事件名)</param>
        /// <param name="Action<Message>">监听事件回调</param>
        public void AddListener(string MessageName, Action<Message> messageEvent)
        {
            // Debug.Log(MessageName);
            List<Action<Message>> list = null;
            if (DicCacheMessageEvents.ContainsKey(MessageName))
            {
                foreach (var item in DicCacheMessageEvents[MessageName])
                {
                    messageEvent(item);
                }
                DicCacheMessageEvents.Remove(MessageName);
            }

            if (DicMessageEvents.ContainsKey(MessageName))
                list = DicMessageEvents[MessageName];
            else
            {
                list = new List<Action<Message>>();
                DicMessageEvents.Add(MessageName, list);
            }

            if (!list.Contains(messageEvent))
                list.Add(messageEvent);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="messageType">需要移除的监听类型</param>
        /// <param name="messageEvent">需要移除的事件</param>
        public void RemoveListener(MessageType messageType, Action<Message> messageEvent)
        {
            RemoveListener(messageType.ToString(), messageEvent);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="MessageName">需要移除的监听名</param>
        /// <param name="messageEvent">需要移除的事件</param>
        public void RemoveListener(string MessageName, Action<Message> messageEvent)
        {
            if (DicMessageEvents.ContainsKey(MessageName))
            {
                List<Action<Message>> list = DicMessageEvents[MessageName];
                if (list.Contains(messageEvent))
                {
                    list.Remove(messageEvent);
                }
                else if (list.Count >= 0)
                {
                    DicMessageEvents.Remove(MessageName);
                }
            }
        }
        /// <summary>
        /// 移除所有监听
        /// </summary>
        public void RemoveAllListener()
        {
            DicMessageEvents.Clear();
        }

        #endregion

        #region 发送消息

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">封装后的消息类对象</param>
        ///  <param name="isCache">是否开启缓存(默认开启)</param>
        public void SendMessage(Message message, bool isCache = true)
        {
            DoMessageDispatcher(isCache, message);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="name">消息名</param>
        /// <param name="sender">消息内容</param>
        /// <param name="content">附加的内容</param>
        /// <param name="isCache">是否开启缓存(默认开启)</param>
        /// <param name="_params">可变参数</param>
        public void SendMessage(string name, object sender, object content, bool isCache = true, params object[] _params)
        {
            SendMessage(new Message(name, sender, content, _params), isCache);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="name">消息名</param>
        /// <param name="sender">消息内容</param>
        /// <param name="content">附加的内容</param>
        /// <param name="isCache">是否开启缓存(默认开启)</param>
        public void SendMessage(string name, object sender, object content, bool isCache = true)
        {
            SendMessage(name, sender, content, isCache, null);
        }

        /// <summary>
        /// 发送消息 
        /// </summary>
        /// <param name="name">消息名</param>
        /// <param name="sender">消息内容</param>
        /// <param name="isCache">是否开启缓存(默认开启)</param>
        public void SendMessage(string name, object sender, bool isCache = true)
        {
            SendMessage(name, sender, null, isCache);
        }

        private void DoMessageDispatcher(bool isCache, Message message)
        {
            // Debug.Log("DoMessageDispatcher Name:" + message.Name);
            if (DicMessageEvents == null)
                return;
            if (isCache)
            {
                // Debug.Log("开启消息缓存");
                if (!DicMessageEvents.ContainsKey(message.Name))
                {
                    if (!DicCacheMessageEvents.ContainsKey(message.Name))
                        DicCacheMessageEvents.Add(message.Name, new List<Message>());

                    DicCacheMessageEvents[message.Name].Add(message);
                }
                else
                {
                    AddMessageEvents(message);
                }
                //Debug.Log("消息中" + DicMessageEvents.ContainsKey(message.Name) + "包含" + message.Name);
                //Debug.Log("缓存中" + DicCacheMessageEvents.ContainsKey(message.Name) + "包含" + message.Name);
            }
            else
            {
                // Debug.Log("关闭消息缓存");
                AddMessageEvents(message);
            }
        }

        private void AddMessageEvents(Message message)
        {
            List<Action<Message>> list = DicMessageEvents[message.Name];

            for (int i = 0; i < list.Count; i++)
            {
                list[i]?.Invoke(message);
            }
        }

        #endregion
    }
}
