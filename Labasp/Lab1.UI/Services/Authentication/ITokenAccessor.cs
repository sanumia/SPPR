public interface ITokenAccessor
{
    Task<string?> GetUserAccessTokenAsync();
    Task SetAuthorizationHeaderAsync(HttpClient client, bool isClient);
}
