using System;
using System.Collections.Generic;
using System.Text;

namespace JavaCompiler
{
    public class Token
    {
        public Resources.Symbol token { get; set; }
        public string attribute { get; set; }

        public Token(Resources.Symbol _token, string _attr)
        {
            token = _token;
            attribute = _attr;
        }
    }
}
