using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public static class DbConnection
    {
        private static string _connectionString =
            "Server=.;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static void Configure(string connectionString)
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
                _connectionString = connectionString;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
