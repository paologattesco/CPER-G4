using ITS.CPER.DataDequeue.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.CPER.DataDequeue.Service;

public interface IDataAccess
{
    void InsertInfluxDb(SmartWatch_Data data);
    void InsertSqlManagement(SmartWatch_Data data);

}
