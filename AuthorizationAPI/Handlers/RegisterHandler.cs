using System.Text.Json;
using AuthorizationAPI.Commands;
using AuthorizationAPI.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProfilesAPI.Models;

namespace AuthorizationAPI.Handlers;

public class RegisterHandler : IRequestHandler<RegisterCommand>
{
    private readonly HttpClient _httpClient;
    private readonly UserManager<Account> _userManager;

    public RegisterHandler(HttpClient httpClient, UserManager<Account> userManager)
    {
        _httpClient = httpClient;
        _userManager = userManager;
    }
    
    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (request.RegisterViewModel.IsDoctor)
        {
            var httpResponseMessage = await _httpClient.GetAsync("?id=" + request.RegisterViewModel, cancellationToken);
            
            if (!httpResponseMessage.IsSuccessStatusCode) 
                throw new HttpRequestException("ProfilesAPI returned code: " + httpResponseMessage.StatusCode);
            
            await using var contentStream =
                await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
            
            var doctor = await JsonSerializer.DeserializeAsync
                <Doctor>(contentStream, cancellationToken: cancellationToken, 
                    options: new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });

            httpResponseMessage = await _httpClient.PatchAsync("");
        }
        
        var user = new Account 
        { 
        Email = request.RegisterViewModel.Email, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now};
        var result = await _userManager.CreateAsync(user, request.RegisterViewModel.Password);
    }
}