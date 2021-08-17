using System;
using System.Collections.Generic;
using System.Text;

namespace AkaDB.MySql
{
    public class InputArg
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public InputArg(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
