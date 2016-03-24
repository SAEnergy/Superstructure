using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Core.Comm
{
    public class BindingInformation
    {
        // options would go here if there were any

        public static Binding BuildBinding(BindingInformation info, ServerConnectionInformation server)
        {
            var binding = new NetTcpBinding(SecurityMode.Transport, false);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;

            return binding;
        }
    }
}
