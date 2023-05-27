using InfluxDB.Client;
using ITS.CPER.WebPage.Data.Models;

namespace ITS.CPER.WebPage.Data.Services;

public interface IDataAccess
{
    void InsertNewUser(Guid id);
    Task<SmartWatch_Data>GetSmartWatchDataAsync(Guid id);
    Task<SmartWatch_Data> HeartbeatQuery(SmartWatch_Data data);

    Guid GetUserId(string userName);
}
