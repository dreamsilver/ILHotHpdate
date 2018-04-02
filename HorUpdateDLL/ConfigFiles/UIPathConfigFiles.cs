using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotUpdateDLL
{
    public class UIPathConfigFiles
    {
        public List<PathDic> listPath = new List<PathDic>();
    }

    public class PathDic
    {
        public string Name;
        public string Path;
    }
}
