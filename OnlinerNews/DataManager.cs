using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinerNews
{
    public static class DataManager<T> where T : new()
    {
        // RTFM : http://csharpindepth.com/articles/general/singleton.aspx


        private static readonly Lazy<T> lazy = new Lazy<T>(() => new T());
        public static T Instance { get { return lazy.Value; } }

    }
}
