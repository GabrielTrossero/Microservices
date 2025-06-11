using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ProductsService.Event;
using ProductsService.Models;
using Microsoft.Extensions.DependencyInjection;
using ProductsService.Data;
using ProductsService.Services;

namespace ProductsService.Messaging
{
    public class EventBusConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public EventBusConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ? "rabbitmq" : "localhost"             // Detecta si está corriendo dentro de un contenedor
            };

            _connection = factory.CreateConnection(); // Creamos la conexion
            _channel = _connection.CreateModel(); // Creamos el canal
            _channel.ExchangeDeclare(exchange: "user_events", type: ExchangeType.Fanout); // Declaramos el exchange

            var queueName = _channel.QueueDeclare().QueueName; // Creamos una cola
            _channel.QueueBind(queue: queueName, exchange: "user_events", routingKey: ""); // Asociamos la cola al canal



            var consumer = new EventingBasicConsumer(_channel); // Tengo el consumer
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray(); // Obtnemos el array de bytes
                var message = Encoding.UTF8.GetString(body); // Transformo de bytes a string

                // Procesar el mensaje, por ejemplo, deserializar el evento y procesarlo
                var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message);

                if (userCreatedEvent != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                        // Usar el servicio para asignar un producto predeterminado al usuario
                        await productService.AssignDefaultProductToUser(userCreatedEvent.Id, userCreatedEvent.Nombre);
                    }
                }

            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer); // Confirmamos que recibimos el evento
        }
    }
}
