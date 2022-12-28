using AuthorizationAPI.Commands;
using AuthorizationAPI.Data;
using AuthorizationAPI.Infrastructure;
using AuthorizationAPI.Models;
using AuthorizationAPI.RabbitMq;
using AuthorizationAPI.ViewModels;
using CommunicationModels;
using MediatR;
using Raven.Client.Documents;

namespace AuthorizationAPI.Handlers;

public class RegisterHandler : IRequestHandler<RegisterCommand>
{
    private readonly RavenDbContext _dbContext;
    private readonly IPasswordHashGenerator _passwordHashGenerator;
    private readonly IRabbitMqService _rabbitMq;
    private readonly RegisterValidator _registerValidator;

    public RegisterHandler(IPasswordHashGenerator passwordHashGenerator, RavenDbContext dbContext, 
       IRabbitMqService rabbitMq, RegisterValidator registerValidator)
    {
        _passwordHashGenerator = passwordHashGenerator;
        _dbContext = dbContext;
        _rabbitMq = rabbitMq;
        _registerValidator = registerValidator;
    }
    
    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _registerValidator.ValidateAsync(request.RegisterViewModel, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new Exception(validationResult.Errors.First().ErrorMessage);
        }
        
        var store = _dbContext.Store;
        using var session = store.OpenAsyncSession();
        
        var account = await session.Query<Account>()
            .Search(a => a.Email, request.RegisterViewModel.Email)
            .FirstOrDefaultAsync(cancellationToken);

        if (account is not null)
            throw new Exception("User with this email already exists");

        account = new Account
        {
            Id = Guid.NewGuid().ToString(),
            PasswordHash = _passwordHashGenerator.GenerateHash(request.RegisterViewModel.Password),
            PhoneNumber = null,
            Email = request.RegisterViewModel.Email,
            IsEmailVerified = true,
            PhotoId = null,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        await session.StoreAsync(account, cancellationToken);
        await session.SaveChangesAsync(cancellationToken);

        var x = new Patient
        {
            Id = Guid.NewGuid().ToString(),
            AccountId = account.Id
        };
        _rabbitMq.SendMessage(x);

        return Unit.Value;
    }
}