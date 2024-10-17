using System.Diagnostics;

namespace OnlineStore.WebAPI.Utilities
{
    public class ConfigurationHelper
    {
        private static IConfiguration _config;

        public static string? JwtKey { get; set; }

        public static IConfiguration? Configuration
        {
            get
            {
                Debug.WriteLine("Inside get of ConfigurationHelper");
                return _config;
            }
            set
            {
                _config = value;
            }
        }

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
