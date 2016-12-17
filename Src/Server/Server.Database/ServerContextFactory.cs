using Core.Interfaces.Components.IoC;
using Server.Components;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database
{
    [ComponentRegistration(typeof(IDatabaseContextFactory))]
    public class ServerContextFactory : IDbContextFactory<ServerContext>, IDatabaseContextFactory
    {
        public static string ConnectionString { get; set; }

        public static ServerContextFactory Instance { get; private set; } = new ServerContextFactory();

        public ServerContext Create()
        {
            return NewContext();
        }

        public ServerContext NewContext(string connString)
        {
            //ConnectionString = "Provider=System.Data.SqlClient; Provider Connection String='" + connString + "';";
            ConnectionString = connString;
            return new ServerContext(ConnectionString);
        }

        public ServerContext NewContext()
        {
            return new ServerContext(ConnectionString);
        }

        public ServerContext ProxyContext()
        {
            ServerContext context = new ServerContext(ConnectionString);
            context.Configuration.ProxyCreationEnabled = false;
            return context;
        }

        DbContext IDatabaseContextFactory.NewContext()
        {
            return new ServerContext(ConnectionString);
        }
    }
}