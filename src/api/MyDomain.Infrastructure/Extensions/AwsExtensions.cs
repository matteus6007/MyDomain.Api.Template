using Amazon.Runtime;

namespace MyDomain.Infrastructure.Extensions
{
    public static class AwsExtensions
    {
        public static BasicAWSCredentials GetTestCredentials() => new("test", "test");
    }
}