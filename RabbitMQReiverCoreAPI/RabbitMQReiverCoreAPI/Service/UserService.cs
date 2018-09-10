using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQReiverCoreAPI.Models;

namespace RabbitMQReiverCoreAPI.Service
{
    public class UserService
    {
        internal UserSaveFeedback InsertUsers(List<User> userList)
        {
            List<User> failList = new List<User>();
            foreach (User user in userList)
            {
                failList.Add(user);
                Thread.Sleep(1000);//fake waiting 
            }

            UserSaveFeedback saveFeedback = new UserSaveFeedback();
            saveFeedback.successCount = 1;
            saveFeedback.failedCount = 1;
            saveFeedback.failedList= failList;//Add fake failed Items

            return saveFeedback;
        }
    }
}
