using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace UsersService.Messaging
{
    public class EventBusPublisher
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public EventBusPublisher()
        {
            var factory = new ConnectionFactory();

            // Detecta si está corriendo dentro de un contenedor
            var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
            factory.HostName = isDocker ? "rabbitmq" : "localhost";

            _connection = factory.CreateConnection(); // Creamos una conexion
            _channel = _connection.CreateModel(); // Creamos un canal

            _channel.ExchangeDeclare(exchange: "user_events", type: ExchangeType.Fanout); // Exchange recibe mensajes y determina a que colas se envian luego. Es el "cartero"
            // Existen varios tipos. En este caso usamos Fanout: Envía el mensaje a todas las colas vinculadas sin importar la routing key
        }

        public void PublishUserCreated(object message)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)); // Rabbit usa bytes para comunicar la info. No usa string, int, etc.

            _channel.BasicPublish( // Publicamos el evento
                exchange: "user_events",
                routingKey: "",
                basicProperties: null,
                body: body
            );
        }

    }
}
