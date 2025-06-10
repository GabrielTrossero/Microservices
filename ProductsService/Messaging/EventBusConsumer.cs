using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ProductsService.Event;

namespace ProductsService.Messaging
{
    public class EventBusConsumer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public EventBusConsumer()
        {
            var factory = new ConnectionFactory { HostName = "rabbitmq" };

            _connection = factory.CreateConnection(); // Creamos la conexion
            _channel = _connection.CreateModel(); // Creamos el canal

            _channel.ExchangeDeclare(exchange: "product_events", type: ExchangeType.Fanout); // Declaramos el exchange

            var queueName = _channel.QueueDeclare().QueueName; // Creamos una cola
            _channel.QueueBind(queue: queueName, exchange: "product_events", routingKey: ""); // Asociamos la cola al canal

            Console.WriteLine($"Esperando mensajes en {queueName}...");

            var consumer = new EventingBasicConsumer(_channel); // Tengo el consumer
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray(); // Obtnemos el array de bytes
                var message = Encoding.UTF8.GetString(body); // Transformo de bytes a string

                // Procesar el mensaje, por ejemplo, deserializar el evento y procesarlo
                var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message);
                Console.WriteLine($"Usuario creado recibido: {userCreatedEvent.Name}, {userCreatedEvent.Email}");

                // Lógica para manejar el evento recibido (por ejemplo, guardar en la base de datos)
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer); // Confirmamos que recibimos el evento
        }
    }
}
