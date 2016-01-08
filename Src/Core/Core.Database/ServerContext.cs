using Core.Models.Persistent;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Database
{
    public class ServerContext : DbContext
    {
        #region Properties

        public DbSet<User> Users { get; set; }

        #endregion

        #region Constructors

        public ServerContext() : base(DatabaseSettings.Instance.ConnectionString) { }

        //main constructor
        public ServerContext(string connectionString) : base(connectionString)
        {
            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<ServerContext, ServerContextConfig>());
        }

        #endregion

        public override int SaveChanges()
        {

            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
