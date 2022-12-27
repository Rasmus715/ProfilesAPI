using System.Text;
using System.Text.Json;
using CommunicationModels;
using RabbitMQ.Client;
using Patient = CommunicationModels.Patient;

namespace AuthorizationAPI.RabbitMq;

public interface IRabbitMqService
{
    void SendMessage(RabbitLog log);
    void SendMessage(Patient patient);
}

public class RabbitMqService : IRabbitMqService
{
    private readonly IModel _model;
    
    public RabbitMqService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        
        var connection = factory.CreateConnection();
        _model = connection.CreateModel();
    }
    
    public void SendMessage(RabbitLog log)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(log));
        _model.BasicPublish(
            "logsExchange",
            "",
            null,
            body);
    }

    public void SendMessage(Patient patient)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(patient));

        _model.BasicPublish(
            "ProfilesExchange",
            "Patients",
            null,
            body);
    }
}