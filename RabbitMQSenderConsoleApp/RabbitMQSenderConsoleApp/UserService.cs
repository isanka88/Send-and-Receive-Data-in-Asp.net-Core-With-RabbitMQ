using System.Collections.Generic;

namespace RabbitMQSenderConsoleApp
{
    internal class UserService
    {
        public UserService()
        {
        }

        internal List<User> GetAllUsersToSend()
        {
            List<User> userList = new List<User>();
            userList.Add(new User { FirstName = "Isanka", LastName = "Thalagala", EmailAddress = "isanka.thalagala@gmail.com" });
            userList.Add(new User { FirstName = "Ish", LastName = "Thalagala", EmailAddress = "isanka.thalagala@outsmarthub.co.nz" });

            return userList;
        }
    }
}