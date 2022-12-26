using AuthorizationAPI.Commands;
using AuthorizationAPI.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationAPI.Handlers;

public class RegisterHandler : IRequestHandler<RegisterCommand>
{
    private readonly HttpClient _httpClient;

    public RegisterHandler(HttpClient httpClient, UserManager<Account> userManager)
    {
        _httpClient = httpClient;
    }
    
    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
      
            // var httpResponseMessage = await _httpClient.GetAsync("?id=" + request.RegisterViewModel, cancellationToken);
            //
            // if (!httpResponseMessage.IsSuccessStatusCode) 
            //     throw new HttpRequestException("ProfilesAPI returned code: " + httpResponseMessage.StatusCode);
            //
            // await using var contentStream =
            //     await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
            //
            // var doctor = await JsonSerializer.DeserializeAsync
            //     <Doctor>(contentStream, cancellationToken: cancellationToken, 
            //         options: new JsonSerializerOptions
            //         {
            //             PropertyNameCaseInsensitive = true,
            //         });
            //
            // httpResponseMessage = await _httpClient.PatchAsync("");
        
        var user = new Account
        {
            Id = default,
            PasswordHash = null,
            PhoneNumber = null,
            Email = null,
            IsEmailVerified = false,
            PhotoId = null,
            CreatedAt = null,
            UpdatedAt = null
        };
    }
}