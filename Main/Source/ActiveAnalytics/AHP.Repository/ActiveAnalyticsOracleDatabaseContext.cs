using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHP.Core.Model;

namespace AHP.Repository
{
    public class ActiveAnalyticsOracleDatabaseContext: System.Data.Entity.DbContext
    {
        //public System.Data.Entity.DbSet<PersonalInfoQuestion> PersonalInfoQuestions { get; set; }

        public DbSet<UserSecurityAnswer> SecurityQuestionAnswer { get; set; }

        public DbSet<SecurityInfoQuestion> SecurityQuestions { get; set; }

        public System.Data.Entity.DbSet<UserInfo> UserInfo { get; set; }

        public DbSet<EventAudit> AuditInfo { get; set; }

        public DbSet<InternalUserTableauInfo> InternalUserInfo { get; set; }

        public DbSet<WorkbookViewInfo> WorkbookViews { get; set; }

        public DbSet<TableauViewsForUser> ViewsForUser { get; set; }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(ConfigurationManager.AppSettings["DefaultSchema"]);
            base.OnModelCreating(modelBuilder);
        }

        public ActiveAnalyticsOracleDatabaseContext():base("ActiveAnalyticsOracleDatabaseContext")
        {            
        }
    }
}
