namespace OnlineStore.WebAPI.Utilities
{
    public class ConfigurationHelper
    {
        public static string? JwtKey { get; set; }

        public static IConfiguration? Configuration { get; set; }

        public static string? GetConfigValue(string key)
        {
            if (Configuration is null)
            {
                return null;
            }

            return Configuration[key];
        }
    }
}
