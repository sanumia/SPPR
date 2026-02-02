namespace Lab1.API.HelperClasses
{
    public class KeycloakData
    {
        public string Host { get; set; } = string.Empty;
        public string Realm { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        
        // Для Admin API можно использовать service account
        // Если не указаны, используются обычные ClientId и ClientSecret
        public string? AdminClientId { get; set; }
        public string? AdminClientSecret { get; set; }
    }
}