using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQReiverCoreAPI.Models
{
    public class UserSaveFeedback
    {
        public int successCount { get; set; }
        public int failedCount { get; set; }
        public List<User> failedList { get; set; }
    }
}
