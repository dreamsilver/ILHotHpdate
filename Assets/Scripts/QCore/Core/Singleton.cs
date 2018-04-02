using System;
namespace QCore.Core
{
    /// <summary>
    /// 单例模式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Type t = typeof(T);
                    // 获取所有构造函数
                    var constructors = t.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

                    // 查找空参构造
                    System.Reflection.ConstructorInfo constructor = null;
                    foreach (var item in constructors)
                    {
                        if(item.GetParameters().Length == 0)
                        {
                            constructor = item;
                            break;
                        }
                    }

                    if(constructor == null)
                    {
                        throw new NotSupportedException("没有 0 个参数的构造函数");
                    }
                    _instance = constructor.Invoke(null) as T;
                }
                return _instance;
            }
        }
    }
}