using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQSenderConsoleApp
{
    class Program
    {

        //packege manager console > Install-Package RabbitMQ.Client
        //packege manager console > Install-Package Newtonsoft.json

        static void Main(string[] args)
        {
            Console.WriteLine("Hello this is the sender application!");
            string senderUniqueId = "userInsertMsgQ";

            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            var connection = factory.CreateConnection();

            //-------------------------  Sending Data --------------------------------------------------------------------------------------
            #region Sending Data
            using (var channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: "userInsertMsgQ", durable: false, exclusive: false, autoDelete: false, arguments: null);

                Console.WriteLine("Type something and press 'Enter Button' to send user list");
                Console.ReadLine();

                // create serialize object to send
                UserService _userService = new UserService();
                List<User> objeUserList = _userService.GetAllUsersToSend();
                string message = JsonConvert.SerializeObject(objeUserList);

                var body = Encoding.UTF8.GetBytes(message);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;
                properties.Headers = new Dictionary<string, object>();
                properties.Headers.Add("senderUniqueId", senderUniqueId);//optional unique sender details in receiver side              
                // properties.Expiration = "36000000";
                //properties.ContentType = "text/plain";

                channel.ConfirmSelect();

                channel.BasicPublish(exchange: "",
                                     routingKey: "userInsertMsgQ",
                                     basicProperties: properties,
                                     body: body);

                channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, eventArgs) =>
                {
                    Console.WriteLine("Sent RabbitMQ");
                    //implement ack handle
                };
                channel.ConfirmSelect();

            }
            #endregion


            //-------------------------  Receiving feedback ---------------------------------------------------------------------------------
            #region Feedback Received part
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "userInsertMsgQ_feedback",
                                   durable: false,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    IDictionary<string, object> headers=ea.BasicProperties.Headers; // get headers from Received msg

                    foreach (KeyValuePair<string, object> header in headers)
                    {                      
                        if (senderUniqueId == Encoding.UTF8.GetString((byte[])header.Value))// Get feedback message only for me
                        {
                            var body = ea.Body;
                           var message = Encoding.UTF8.GetString(body);
                            UserSaveFeedback feedback = JsonConvert.DeserializeObject<UserSaveFeedback>(message);
                            Console.WriteLine("[x] Feedback received ... ");
                            Console.WriteLine("[x] Success count {0} and failed count {1}", feedback.successCount, feedback.failedCount);
                       }
                    }
                };

                channel.BasicConsume(queue: "userInsertMsgQ_feedback",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine("Waiting for feedback. Press [enter] to exit.");
                Console.ReadLine();


            }
            #endregion
        }
    }
}
