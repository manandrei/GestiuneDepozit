using GestiuneDepozit.Data.Models;
using GestiuneDepozit.Helpers;
using System.IO;
using System.Text.Json;

namespace GestiuneDepozit
{
    public static class Configuration
    {
        private static readonly string ConfigFilename;
        public static bool ConfigFileLoaded { get; set; } = false;
        public static Parameters Parameters { get; set; }

        public static string MSSQL_ConnectionString()
        {
            if (ConfigFileLoaded)
            {
                return $"Server={ Configuration.Parameters.ServerAddress.Decrypt() };Database={ Configuration.Parameters.Database.Decrypt() };{ (Configuration.Parameters.IsTrustedConnection ? "Trusted_Connection=True;" : $"User Id={Configuration.Parameters.Username.Decrypt()};Password={Configuration.Parameters.Password.Decrypt()};") };TrustServerCertificate=True";
            }
            return "";
        }

        static Configuration()
        {
            string applocation = Path.GetDirectoryName(typeof(Configuration).Assembly.Location);
            ConfigFilename = Path.Combine(applocation, "configuration.json");

            if (Directory.Exists(applocation))
            {
                if (File.Exists(ConfigFilename))
                {
                    ReadConfigFile();                    
                }
                else
                {
                    if (Parameters == null)
                    {
                        Parameters = new Parameters
                        {
                            AcceptedEULA = false,
                            FirstConfiguration = false,
                            ServerAddress = "localhost".Encrypt(),
                            Database = "GestiuneDepozit".Encrypt(),
                            IsTrustedConnection = true,
                            Username = "username".Encrypt(),
                            Password = "password".Encrypt()
                        };
                    }
                    WriteConfigFile(true);
                }
            }
            else
            {
                Directory.CreateDirectory(applocation);
                WriteConfigFile();
            }
        }

        /// <summary>
        /// Writes file configuration
        /// </summary>
        /// <param name="IsConfigFileDefault">Default to false, if set to true it will set that the file was not configured initially and it has the default values</param>
        public static void WriteConfigFile(bool IsConfigFileDefault = false)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string jsonString = JsonSerializer.Serialize(Parameters, serializerOptions);
            File.WriteAllText(ConfigFilename, jsonString);
            ConfigFileLoaded = !IsConfigFileDefault;
        }

        /// <summary>
        /// Reads file configuration
        /// </summary>
        public static void ReadConfigFile()
        {
            string jsonString = File.ReadAllText(ConfigFilename);
            Parameters = JsonSerializer.Deserialize<Parameters>(jsonString);
            ConfigFileLoaded = true;
            if (string.IsNullOrEmpty(MSSQL_ConnectionString()))
            {
                ConfigFileLoaded = false;
            }
            else
            {
                ConfigFileLoaded = true;
            }            
        }
    }
}
