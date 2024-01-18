namespace MyDomain.Api.Authorization
{
    /// <summary>
    /// MyDomain scopes
    /// </summary>
    public sealed class Scopes
    {
        private const string Audience = "mydomain-api";

        /// <summary>
        /// Read scope
        /// </summary>
        public static string Read = $"{Audience}:read";

        /// <summary>
        /// Write scope
        /// </summary>
        public static string Write = $"{Audience}:write";
    }
}
