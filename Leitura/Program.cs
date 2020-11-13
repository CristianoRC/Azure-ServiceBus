using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace ServiceBus_Example
{
    public class Program
    {
        const string QueueName = "emails";
        static IQueueClient queueClient;

        public static async Task Main(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("SERVICEBUS");
            Console.WriteLine(connectionString);
            queueClient = new QueueClient(connectionString, QueueName);
            ConfigureHandler();
            Console.ReadKey();
            await queueClient.CloseAsync();
        }

        static void ConfigureHandler()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessage, messageHandlerOptions);
        }

        static async Task ProcessMessage(Message message, CancellationToken token)
        {
            Console.WriteLine($"ID:{message.SystemProperties.SequenceNumber}");
            Console.WriteLine($"Corpo:{Environment.NewLine}{Encoding.UTF8.GetString(message.Body)}");
            Console.WriteLine("------------------");
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        static Task ExceptionHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}