using System;
using System.Configuration;

namespace ClientSideApplication.Tools
{
    public class ConfigSettings
    {
        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                var result = appSettings[key] ?? "Not Found";
                if (result.Length == 0)
                {
                    result = "Empty";
                }
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
            return null;
        }
    }
}