using AHP.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Repository
{
    public class AuditEventManager : IAuditEventManager
    {
        
        public void AuditEvent(string eventType, string username, string description)
        {
            try
            {
                using (ActiveAnalyticsOracleDatabaseContext dbContext = new ActiveAnalyticsOracleDatabaseContext())
                {
                    EventAudit auditInfo = new EventAudit();
                    auditInfo.EventId = Guid.NewGuid().ToString();
                    auditInfo.EventDate = DateTime.Now;
                    auditInfo.EventType = eventType;
                    auditInfo.Username = username;
                    auditInfo.Description = description;
                    dbContext.AuditInfo.Add(auditInfo);
                    dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                //eat any exception. Consumer should not care if audit save's fail
            }          
        }
    }
}
