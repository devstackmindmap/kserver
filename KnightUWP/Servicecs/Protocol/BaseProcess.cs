using AkaDB.MySql;
using CommonProtocol;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs.Protocol
{
    public abstract class BaseProcess
    {
        public abstract void OnResponse<TContext>(TContext context, BaseProtocol responseData);


        public abstract BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data);

        public virtual BaseProcess OnPreResponse<TContext>(TContext context, BaseProtocol resData)
        {
            return this;
        }

    }
}
