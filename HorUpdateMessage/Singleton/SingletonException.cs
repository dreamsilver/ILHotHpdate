using System;
using System.Collections.Generic;

namespace HotUpdateMessage
{
     class SingletonException : Exception
    {
        public SingletonException(string msg) : base(msg) { }
    }
}
