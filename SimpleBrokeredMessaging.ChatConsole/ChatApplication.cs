using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.ChatConsole
{
    class ChatApplication
    {
        //ToDo: Enter a valid Service Bus connection string
        static string ConnectionString = "";
        static string TopicPath = "chattopic";


        static void Main(string[] args)
        {
            Console.WriteLine("Enter name:");
            var userName = Console.ReadLine();

            // Create a management client to manage artifacts
            var manager = new ManagementClient(ConnectionString);

            // Create a topic if it does not exist            
            if (!manager.TopicExistsAsync(TopicPath).Result)
            {
                manager.CreateTopicAsync(TopicPath).Wait();
            }

            // Create a subscription for the user
            var description = new SubscriptionDescription(TopicPath, userName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            };            
            manager.CreateSubscriptionAsync(description).Wait();

            
            // Create clients
            var topicClient = new TopicClient(ConnectionString, TopicPath);
            var subscriptionClient = new SubscriptionClient(ConnectionString, TopicPath, userName);

            // Create a message pump for receiving messages
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, ExceptionReceivedHandler);

            // Send a message to say you are here
            var helloMessage = new Message(Encoding.UTF8.GetBytes("Has entered the room..."));
            helloMessage.Label = userName;
            topicClient.SendAsync(helloMessage).Wait();

            while (true)
            {
                string text = Console.ReadLine();
                if (text.Equals("exit")) break;

                // Send a chat message
                var chatMessage = new Message(Encoding.UTF8.GetBytes(text));
                chatMessage.Label = userName;
                topicClient.SendAsync(chatMessage).Wait();

            }

            // Send a message to say you are leaving
            var goodbyeMessage = new Message(Encoding.UTF8.GetBytes("Has left the building..."));
            goodbyeMessage.Label = userName;
            topicClient.SendAsync(goodbyeMessage).Wait();

            // Close the clients
            topicClient.CloseAsync().Wait();
            subscriptionClient.CloseAsync().Wait();

        }


        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Deserialize the message body.
            var text = Encoding.UTF8.GetString(message.Body);
            Console.WriteLine($"{ message.Label }> { text }");
        }



        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
        }




    }
}
