using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParking.Core.Entities
{
    public class ChatbotLog
    {
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
    }

}
