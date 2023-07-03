using InfluxDB.Client;
using ITS.CPER.WebPage.Data.Models;

namespace ITS.CPER.WebPage.Data.Services;

public interface IDataAccess
{
    Guid InsertNewUser(Guid id);
    Task<List<SmartWatch_Data>> GetSmartWatchDataAsync(Guid id);
    Task<List<Heartbeat_Data>> HeartbeatQuery(SmartWatch_Data data);
    Guid GetUserId(string userName);
    void InsertProductionBatch(Guid smartwatchId);

}
