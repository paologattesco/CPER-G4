using Microsoft.Data.SqlClient;
using ITS.CPER.WebPage.Data.Models;
using Dapper;

namespace ITS.CPER.WebPage.Data.Services;

public class DataAccess : IDataAccess
{
    private readonly string _connectionDb;
    public DataAccess(IConfiguration configuration)
    {
        _connectionDb = configuration.GetConnectionString("DbConnection");
    }

    public async Task<IEnumerable<SmartWatch_Data>> GetSmartWatchDataAsync()
    {
        using var connection = new SqlConnection(_connectionDb);
        connection.Open();
        SqlCommand sql = connection.CreateCommand();
        sql.CommandText = @"
            SELECT [SmartWatch_Id]
                ,[Activity_Id]
                ,[Initial_Latitude]
                ,[Initial_Longitude]
                ,[Distance]
                ,[NumberOfPoolLaps]
                ,[Final_Latitude]
                ,[Final_Longitude]
            FROM [dbo].[SmartWatches]
            ";
        sql.ExecuteNonQuery();
        var result = new List<SmartWatch_Data>();
        using (SqlDataReader reader = sql.ExecuteReader())
        {
            while (reader.Read())
            {
                SmartWatch_Data smartWatch = new SmartWatch_Data
                {
                    SmartWatch_Id = Guid.Parse((string)reader["SmartWatch_Id"]),
                    Activity_Id = Guid.Parse((string)reader["Activity_Id"]),
                    Initial_Latitude = Convert.ToDouble(reader["Initial_Latitude"]),
                    Initial_Longitude = Convert.ToDouble(reader["Initial_Longitude"]),
                    Distance = Convert.ToDouble(reader["Distance"]),
                    NumberOfPoolLaps = Convert.ToInt32(reader["NumberOfPoolLaps"]),
                    Final_Latitude = Convert.ToDouble(reader["Final_Latitude"]),
                    Final_Longitude = Convert.ToDouble(reader["Final_Longitude"]),
                };
                result.Add(smartWatch);
            }
            reader.Close();
        }
            return result;
    }
}
