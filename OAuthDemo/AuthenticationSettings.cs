namespace OAuthDemo
{
    public class AuthenticationSettings
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AadInstance { get; set; }

        public string Domain { get; set; }

        public string Audience { get; set; }

        public string Tenant { get; set; }

        public string AzureSqlResource { get; set; }

        public string ConnectionString { get; set; }
    }
}
