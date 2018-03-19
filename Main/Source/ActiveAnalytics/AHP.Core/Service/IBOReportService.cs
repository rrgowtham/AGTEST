using AHP.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP.Core.Service
{
    public interface IBOReportService
    {
        ReportCategory GetReportList(BOAuthentication authModel, string defaultFolderId);
        string GetReport(BOAuthentication authModel, string reportId);
    }
}