using System;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.ServiceBus;

namespace Escrita
{
    public class Program
    {
        const string QueueName = "emails";
        static IQueueClient queueClient;

        public static async Task Main(string[] args)
        {
            var faker = new Faker();
            var connectionString = Environment.GetEnvironmentVariable("SERVICEBUS");
            queueClient = new QueueClient(connectionString, QueueName);
            bool sendNewMessage = false;

            do
            {
                await SendNewMessage(faker.Random.String(1000));
                Console.WriteLine($"Mensagem Enviada! {DateTimeOffset.Now.ToString()}");
                sendNewMessage = SendMessageAgain();
                Console.WriteLine("-----------------------");
            }
            while (sendNewMessage);
        }

        static async Task SendNewMessage(string messageText)
        {
            var message = new Message(Encoding.UTF8.GetBytes(messageText));
            await queueClient.SendAsync(message);
        }

        static bool SendMessageAgain()
        {
            Console.Write("Deseja mandar uma nova mensagem?(S/N) ");
            var userAnswer = Console.ReadLine();
            return userAnswer.Trim().ToUpper() == "S";
        }
    }
}
