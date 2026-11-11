using HeavenMariaBeyond.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace HeavenNetCollector.Infrastructure.Providers
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private const string ConfigurationFileName = "database.json";
        private string? _connectionString;

        public string GetConnectionString()
        {
            return LazyInitializer.EnsureInitialized(ref _connectionString, LoadConnectionString);
        }

        private static string LoadConnectionString()
        {
            var configurationPath = Path.Combine(AppContext.BaseDirectory, ConfigurationFileName);

            if (!File.Exists(configurationPath))
            {
                throw new FileNotFoundException(
                    $"Database configuration was not found. Create '{ConfigurationFileName}' next to the executable.",
                    configurationPath);
            }

            DatabaseConfiguration? configuration;
            try
            {
                configuration = JsonSerializer.Deserialize<DatabaseConfiguration>(
                    File.ReadAllText(configurationPath),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException exception)
            {
                throw new InvalidOperationException(
                    $"Database configuration is not valid JSON: '{configurationPath}'.",
                    exception);
            }

            if (configuration is null ||
                string.IsNullOrWhiteSpace(configuration.UserId) ||
                string.IsNullOrWhiteSpace(configuration.Password) ||
                string.IsNullOrWhiteSpace(configuration.Database) ||
                string.IsNullOrWhiteSpace(configuration.Host))
            {
                throw new InvalidOperationException(
                    $"Database configuration must specify UserId, Password, Database, and Host: '{configurationPath}'.");
            }

            var builder = new MySqlConnectionStringBuilder
            {
                UserID = configuration.UserId,
                Password = configuration.Password,
                Database = configuration.Database,
                Server = configuration.Host,
                SslMode = MySqlSslMode.Preferred,
                UseAffectedRows = true
            };
            if (configuration.Port is { } port)
            {
                builder.Port = port;
            }

            return builder.ConnectionString;
        }

        private sealed class DatabaseConfiguration
        {
            public string? UserId { get; set; }
            public string? Password { get; set; }
            public string? Database { get; set; }
            public string? Host { get; set; }
            public uint? Port { get; set; }
        }
    }
}
