using Microsoft.ServiceBus.Messaging;
using RabbitMQ.Client;
using System;
using System.Configuration;

namespace SbProducer
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
            var displayInfo = runAzureSolution ? "*** AZURE PRODUCER ***" : "*** RABBITMQ PRODUCER ***";

            Console.WriteLine(PrintCharacters('*', displayInfo.Length));
            Console.WriteLine(displayInfo);
            Console.WriteLine(PrintCharacters('*', displayInfo.Length));
            Console.WriteLine();

            if (bool.Parse(ConfigurationManager.AppSettings.Get("RunAzureSolution")))
            {
                // Azure
                PublishAzureServiceBusMessage();
            }
            else
            {
                // RabbitMq
                CreateRabbitMessageQueue();
                PublishRabbitMqMessage();
            }

            Environment.Exit(0);
        }

        private static void PublishAzureServiceBusMessage()
        {            
            var connectionString = ConfigurationManager.ConnectionStrings["Microsoft.ServiceBus.ConnectionString"].ConnectionString;           
            var client = QueueClient.CreateFromConnectionString(connectionString);

            var lastKey = ' ';
            var count = 1;

            BrokeredMessage message;

            while (lastKey != 'q')
            {
                var body = $"Message {count++}";
                message = new BrokeredMessage(body);
                client.Send(message);

                Console.WriteLine($"Sent {message.MessageId}");
                lastKey = Console.ReadKey().KeyChar;
            }

            message = new BrokeredMessage("quit");
            client.Send(message);
        }

        private static void PublishRabbitMqMessage()
        {            
            var lastKey = ' ';
            var count = 0;

            while (lastKey != 'q')
            {
                var messageBodyBytes = System.Text.Encoding.UTF8.GetBytes($"Hello, world {++count}");
                _model.BasicPublish("", QueueName, null, messageBodyBytes);

                Console.WriteLine($"Sent {count}");
                lastKey = Console.ReadKey().KeyChar;
            }
        }

        private static void CreateRabbitMessageQueue()
        {
            // docker command to run hosted rabbitmq in container: docker run --rm -it --hostname rabbit-hostname --name rabbit-name -p 5672:5672 -p 8080:15672 rabbitmq:3-management
            _connectionFactory = new ConnectionFactory{ HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest", VirtualHost = "/" };
            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _model.QueueDeclare(QueueName, true, false, false, null);
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
