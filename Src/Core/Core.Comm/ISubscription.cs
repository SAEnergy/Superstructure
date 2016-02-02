using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Comm
{
    public delegate void SubscriptionDisconnectedEvent(ISubscription source, Exception ex);


    public interface ISubscription
    {
        event EventHandler Connected;
        event SubscriptionDisconnectedEvent Disconnected;

        void Start();

        /// <summary>
        /// Synchronously verify if connection is established.
        /// </summary>
        /// <returns>Bool for successful connection.</returns>
        bool Verify();

        void Stop();
    }

    public interface ISubscription<T> : ISubscription
    {
        T Channel { get; }
    }
}
