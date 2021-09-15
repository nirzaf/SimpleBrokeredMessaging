using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Sender
{
    class SenderConsole
    {
        //ToDo: Enter a valid Service Bus connection string
        static string ConnectionString = "";
        static string QueuePath = "demoqueue";


        static void Main(string[] args)
        {

            // Create a queue client
            var queueClient = new QueueClient(ConnectionString, QueuePath);

            // Send some messages
            for (int i = 0; i < 10; i++)
            {
                var content = $"Message: { i }";
                var message = new Message(Encoding.UTF8.GetBytes(content));
                queueClient.SendAsync(message).Wait();
                Console.WriteLine("Sent: " + i);
            }

            // Close the client
            queueClient.CloseAsync().Wait();
            Console.WriteLine("Sent messages...");
            Console.ReadLine();

        }
    }
}
