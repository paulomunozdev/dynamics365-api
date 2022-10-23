using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace JumpStart
{
    [Serializable]
    public class XmsbsException : Exception
    {
        public XmsbsException() : base() { }

        public XmsbsException(string message) : base(message) { }

        public XmsbsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
