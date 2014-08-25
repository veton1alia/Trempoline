using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public static class Locator<T>
         where T : class, new()
    {
        private static T _Instance;

        public static T Instance { get { return _Instance = _Instance ?? new T(); } }
    }
}
