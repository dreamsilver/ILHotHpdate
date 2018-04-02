using System;
using System.Collections.Generic;

namespace HotUpdateDLL
{
    class SingletonException : Exception
    {
        public SingletonException(string msg) : base(msg) { }
    }
}
