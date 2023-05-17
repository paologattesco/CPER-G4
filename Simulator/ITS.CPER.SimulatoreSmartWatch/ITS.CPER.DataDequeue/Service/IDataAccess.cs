using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.CPER.DataDequeue.Service;

public interface IDataAccess
{
    void InsertHeartbeat(Guid id, int Heartbeat);
}
