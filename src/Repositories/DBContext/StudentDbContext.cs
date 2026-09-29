using Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace Repositories.DBContext
{
    public class StudentDbContext
    {
        public IDbConnection Connection { get; }

        public StudentDbContext(IOptions<ConfigSettings> configSettings)
        {
            var value = configSettings.Value.StudentHubDb;

            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException(
                    "Connection string 'ConnectionStrings:StudentHubDb' is missing in appsettings.json.");

            Connection = new SqlConnection(Decode(value));
        }

        // Accepts a base64-encoded connection string, or a plain one as a fallback.
        private static string Decode(string value)
        {
            try
            {
                var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
                if (decoded.Contains('=')) return decoded;
            }
            catch (FormatException)
            {
                // not base64 - treat as plain connection string
            }
            return value;
        }
    }
}
