using System.Text;
using System.Text.Json;
using CommunicationModels;
using RabbitMQ.Client;
using Patient = CommunicationModels.Patient;

namespace ProfilesAPI.RabbitMq;

public interface IRabbitMqService
{
    void SendMessage(RabbitLog log);
    void SendMessage(Patient patient);
}

public class RabbitMqService : IRabbitMqService
{
    public void SendMessage(RabbitLog log)
    {
        var message = JsonSerializer.Serialize(log);
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            "logsExchange",
            "",
            null,
            body);
    }

    public void SendMessage(Patient patient)
    {
        throw new NotImplementedException();
    }
}