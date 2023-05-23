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
        const string query = @"""
            SELECT [SmartWatch_Id]
                ,[Activity_Id]
                ,[Initial_Latitude]
                ,[Initial_Longitude]
                ,[Distance]
                ,[NumberOfPoolLaps]
                ,[Final_Latitude]
                ,[Final_Longitude]
            FROM [dbo].[SmartWatches]
            """;
        using var connection = new SqlConnection(_connectionDb);
        connection.Open();
        return await connection.QueryAsync<SmartWatch_Data>(query);
    }
}
