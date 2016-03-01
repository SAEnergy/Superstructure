using Core.Interfaces.Components.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Components
{
    public interface ISystemConfiguration : IComponentBase
    {
        T GetConfig<T>(string sectionName, string configName);
    }
}
