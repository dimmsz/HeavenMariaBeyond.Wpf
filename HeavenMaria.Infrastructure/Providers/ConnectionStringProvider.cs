using HeavenMaria.Domain.Interfaces;

namespace HeavenNetCollector.Infrastructure.Providers
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private string conStr = "userid = root; password = shikaman954; database = heavennet; Host = 127.0.0.1; SslMode = Preferred;";

        public string GetConnectionString()
        {
            return conStr;
        }
    }
}
