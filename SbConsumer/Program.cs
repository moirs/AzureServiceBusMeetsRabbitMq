using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SbConsumer
{
    class Program
    {
        private static ConnectionFactory _connectionFactory;
        private static IConnection _connection;
        private static IModel _model;
        private const string QueueName = "TestMessagingQueue";

        static void Main(string[] args)
        {
            var runAzureSolution = bool.Parse(ConfigurationManager.AppSettings.Get("RunAzureSolution"));
            var displayInfo = runAzureSolution ? "*** AZURE CONSUMER ***" : "*** RABBITMQ CONSUMER ***";

            Console.WriteLine(PrintCharacters('*', displayInfo.Length));
            Console.WriteLine(displayInfo);
            Console.WriteLine(PrintCharacters('*', displayInfo.Length));
            Console.WriteLine();

            Run(runAzureSolution);
        }

        private static async void Run(bool runAzureSolution)
        {
            if (runAzureSolution)
            {
                // Azure
                await ConsumeAzureServiceBusMessage();
            }
            else
            {
                // RabbitMq
                await CreateRabbitMessageQueue();
                await ConsumeRabbitMqMessage();
            }
        }

        private static Task ConsumeAzureServiceBusMessage()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Microsoft.ServiceBus.ConnectionString"].ConnectionString;
            var client = QueueClient.CreateFromConnectionString(connectionString);

            client.OnMessage(message =>
            {
                var msgBody = message.GetBody<string>();
                Console.WriteLine($"Processing {message.MessageId} with body {msgBody}");
                if (msgBody.Equals("quit"))
                {
                    Console.WriteLine("Exiting application...");
                    Environment.Exit(0);
                }
            });

            Console.ReadLine();

            return Task.CompletedTask;
        }

        private static Task ConsumeRabbitMqMessage()
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (m, ea) =>
            {
                {
                    var msgBody = System.Text.Encoding.Default.GetString(ea.Body);
                    Console.WriteLine($"Processing {ea.BasicProperties.MessageId} with body {msgBody}");
                    if (msgBody.Equals("quit"))
                    {
                        Console.WriteLine("Exiting application...");
                        Environment.Exit(0);
                    }
                }
            };

            _model.BasicConsume(QueueName, true, consumer);

            Console.ReadLine();

            return Task.CompletedTask;
        }

        private static Task CreateRabbitMessageQueue()
        {
            // docker command to run hosted rabbitmq in container: docker run --rm -it --hostname rabbit-hostname --name rabbit-name -p 5672:5672 -p 8080:15672 rabbitmq:3-management
            _connectionFactory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest", VirtualHost = "/" };
            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _model.QueueDeclare(QueueName, true, false, false, null);

            return Task.CompletedTask;
        }

        private static string PrintCharacters(char characterToPrint, int noOfCharactersToPrint)
        {
            var output = string.Empty;

            for (int i = 0; i < noOfCharactersToPrint; i++)
            {
                output += characterToPrint;
            }

            return output;
        }
    }
}
