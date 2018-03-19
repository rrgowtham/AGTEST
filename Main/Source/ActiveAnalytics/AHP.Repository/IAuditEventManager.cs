using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Repository
{
    public interface IAuditEventManager
    {
        void AuditEvent(string eventType, string username, string description);
    }
}
