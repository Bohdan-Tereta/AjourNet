using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjourAPIServer.Types
{
    [Flags]
    public enum BTStatus
    {
        Planned = 0x01,
        Registered = 0x02,
        Confirmed = 0x04,
        Modified = 0x08,
        Cancelled = 0x10,
        Reported = 0x20,
    };
}