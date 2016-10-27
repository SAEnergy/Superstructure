using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Comm
{
    public interface IServiceHost
    {
        Type InterfaceType { get; }
    }
}
