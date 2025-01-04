using Microsoft.Data.SqlClient;

namespace Data.Commands
{
    public class DatabaseStarter
    {
        public string connectionString = string.Empty;
        public DatabaseStarter(string connectionString)
        {
            this.connectionString = connectionString;
            EnsureDatabaseCreated();
        }
        private void EnsureDatabaseCreated()
        {
            CreateDatabase();
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "CreateDatabase.sql");
                    if (File.Exists(scriptPath))
                    {
                        connectionString = connectionString.Replace("database=FastCommerce;", "");
                        var script = File.ReadAllText(scriptPath);
                        using (var connection_2 = new SqlConnection(connectionString))
                        {
                            connection_2.Open();
                            var command = connection_2.CreateCommand();
                            command.CommandText = script;
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch(Exception ex)
                {

                }
            }
        }
        private void CreateDatabase()
        {
            try
            {
                connectionString = connectionString.Replace("database=FastCommerce;", "");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $@"
                            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FastCommerce')
                            BEGIN
                                CREATE DATABASE FastCommerce
                            END;";
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {

            }
        }
    }

}
