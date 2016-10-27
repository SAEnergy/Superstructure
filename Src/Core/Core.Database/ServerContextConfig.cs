using System.Data.Entity.Migrations;

namespace Core.Database
{
    internal sealed class ServerContextConfig : DbMigrationsConfiguration<ServerContext>
    {
        public ServerContextConfig()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true; //set this to false when in deployment maybe?
        }

        protected override void Seed(ServerContext context)
        {
            //  This method will be called after migrating to the latest version.
        }
    }
}
