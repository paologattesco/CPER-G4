using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ITS.CPER.SimulatoreSmartWatch.Service;

public class DataAccess : IDataAccess
{
    private readonly string _connectionString;

    public DataAccess(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("db");
    }

    public List<Guid> GetSmartWatchesId()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var smartwatchesId = new List<Guid>();

        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = $"SELECT [Id] FROM [dbo].[SmartWatches]";
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                smartwatchesId.Add(Guid.Parse((string)reader["Id"]));
            }
            reader.Close();
        }

        return smartwatchesId;
    }

    public Guid GetUserId(Guid smartwatchId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var userId = new Guid();

        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = $"SELECT [FK_User_Id] FROM [dbo].[SmartWatches] WHERE Id = @Id";
        sql.Parameters.AddWithValue("@Id", smartwatchId);
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                userId = Guid.Parse((string)reader["FK_User_Id"]);
            }
            reader.Close();
        }

        return userId;
    }
}