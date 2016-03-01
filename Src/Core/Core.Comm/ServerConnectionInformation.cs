using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Core.Comm
{
    public class ServerConnectionInformation
    {
        public event Action Reconnect;
        public string ConnectionString { get; set; }
        public ClientCredentials AlternateCredentials { get; set; }

        public ServerConnectionInformation(string connectionString, ClientCredentials credentials = null)
        {
            ConnectionString = connectionString;
            AlternateCredentials = credentials;
        }

        public void FireReconnect()
        {
            if (Reconnect!=null) { Reconnect(); }
        }

        // todo: Remove static connection instance once multi-server client architecture is set up
        public static ServerConnectionInformation Instance = new ServerConnectionInformation("localhost");
    }
}
