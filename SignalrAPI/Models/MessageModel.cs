using System.Collections.Generic;

namespace SignalrAPI.Models
{
    public class MessageModel
    {
        //[Required]
        public string EventType { get; set; }
        //[Required]
        public string ServiceType { get; set; }
        public int UserId { get; set; } // supply when need send messae to specific User
        public List<int> UserIds { get; set; } // supply when need send messae to list of User
        public int MessageStatusId { get; set; }// Delivered, Un-read, read

        public object Data { get; set; }
    }
}
