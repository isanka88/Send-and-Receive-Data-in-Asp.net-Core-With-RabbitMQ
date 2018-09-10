using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQSenderConsoleApp
{
    public class UserSaveFeedback
    {
        public int successCount { get; set; }
        public int failedCount { get; set; }
        public List<User> failedList { get; set; }
    }
}
