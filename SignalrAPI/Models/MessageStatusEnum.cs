using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalrAPI.Models
{
    public enum MessageStatusEnum
    {
        Sent = 0,
        Delivered = 1,
        Unread = 2,
        Read = 3,
    }
}
