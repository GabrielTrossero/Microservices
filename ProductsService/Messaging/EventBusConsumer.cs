using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ProductsService.Event;
using ProductsService.Models;
using Microsoft.Extensions.DependencyInjection;
using ProductsService.Data;

namespace ProductsService.Messaging
{
    public class EventBusConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public EventBusConsumer(IServiceProvider serviceProvider)
        {
            var factory = new ConnectionFactory();

            // Detecta si está corriendo dentro de un contenedor
            var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
            factory.HostName = isDocker ? "rabbitmq" : "localhost";

            _connection = factory.CreateConnection(); // Creamos la conexion
            _channel = _connection.CreateModel(); // Creamos el canal

            _channel.ExchangeDeclare(exchange: "user_events", type: ExchangeType.Fanout); // Declaramos el exchange

            var queueName = _channel.QueueDeclare().QueueName; // Creamos una cola
            _channel.QueueBind(queue: queueName, exchange: "user_events", routingKey: ""); // Asociamos la cola al canal

            Console.WriteLine($"Esperando mensajes en {queueName}...");

            var consumer = new EventingBasicConsumer(_channel); // Tengo el consumer
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray(); // Obtnemos el array de bytes
                var message = Encoding.UTF8.GetString(body); // Transformo de bytes a string

                // Procesar el mensaje, por ejemplo, deserializar el evento y procesarlo
                var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message);

                var product = new Product
                {
                    Nombre = "Producto de regalo",
                    Id_User = userCreatedEvent.Id
                };

                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
                    dbContext.Products.Add(product);
                    dbContext.SaveChanges();
                }

            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer); // Confirmamos que recibimos el evento
        }
    }
}
