namespace MyDomain.Api.Options
{
    public class IdentityOptions
    {
        public const string SectionName = "MyDomain:Identity";

        public string Issuer { get; set; }
        public string Audience { get; set; } = "mydomain-api";
        public bool RequireHttpsMetadata { get; set; } = false;
    }
}
