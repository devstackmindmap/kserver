using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs.Protocol
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        public MessageType MessageType;

        private string _name;
        public string Name { get { return _name == null ? MessageType.ToString() : _name; } set { _name = value; } }
    }
}
