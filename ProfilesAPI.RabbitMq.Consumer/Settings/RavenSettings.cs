namespace ProfilesAPI.RabbitMq.Consumer.Settings;

public class RavenSettings
{
    public string[] Urls { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}