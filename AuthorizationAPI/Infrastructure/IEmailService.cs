namespace AuthorizationAPI.Infrastructure;

public interface IEmailService
{
    public Task SendVerificationEmail();
}

public class EmailService : IEmailService
{
    public async Task SendVerificationEmail()
    {
        await Task.FromResult("ok");
    }
}