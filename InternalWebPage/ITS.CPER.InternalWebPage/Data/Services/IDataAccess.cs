using InfluxDB.Client;
using ITS.CPER.InternalWebPage.Data.Models;

namespace ITS.CPER.InternalWebPage.Data.Services;

public interface IDataAccess
{
    void InsertNewUser(Guid id);
    Task<List<SmartWatch_Data>> GetSmartWatchDataAsync(Guid id);
    Task<List<Heartbeat_Data>> HeartbeatQuery(SmartWatch_Data data);

    Guid GetUserId(string userName);
}
