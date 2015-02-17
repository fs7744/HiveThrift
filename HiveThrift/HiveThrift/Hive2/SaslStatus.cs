using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thrift.Transport
{
    public enum SaslStatus
    {
        START = 1,
        OK = 2,
        BAD = 3,
        ERROR = 4,
        COMPLETE = 5
    }
}