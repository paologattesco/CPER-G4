using ITS.CPER.WebPage.Data.Models;

namespace ITS.CPER.WebPage.Data.Services;

public interface IDataAccess
{
    void InsertNewUser(Guid id);
    Task<IEnumerable<SmartWatch_Data>> GetSmartWatchDataAsync();
}
