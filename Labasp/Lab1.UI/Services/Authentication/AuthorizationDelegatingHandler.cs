using Lab1.UI.Services.Authentication;

public class AuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly ITokenAccessor _tokenAccessor;
    private readonly ILogger<AuthorizationDelegatingHandler> _logger;

    public AuthorizationDelegatingHandler(
        ITokenAccessor tokenAccessor,
        ILogger<AuthorizationDelegatingHandler> logger)
    {
        _tokenAccessor = tokenAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Получаем токен напрямую
            var token = await _tokenAccessor.GetUserAccessTokenAsync();
            Console.WriteLine($"Token: {token}");
            if (!string.IsNullOrEmpty(token))
            {

                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogWarning("Authorization header not set: token is null.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set authorization header.");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
