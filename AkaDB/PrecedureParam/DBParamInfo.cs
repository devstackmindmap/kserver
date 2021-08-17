using System;
using System.Collections.Generic;
using System.Text;

namespace AkaDB.MySql
{
    public class DBParamInfo
    {
        public InputArg[] InputArgs { get; set; }
        public OutputArg[] OutputArgs { get; set; }
        public DBContext Sender { get; set; }
        bool isNextResult = true;

        public void SetInputParam(params InputArg[] args)
        {
            InputArgs = args;
        }

        public void SetOutputParam(params OutputArg[] args)
        {
            OutputArgs = args;
        }


        public T GetOutValue<T>(string key)
        {
            if (isNextResult)
            {
                while (Sender.Cursor.NextResult()) { }

                isNextResult = false;
            }

            var value = Sender.Command.Parameters[key].Value;
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
