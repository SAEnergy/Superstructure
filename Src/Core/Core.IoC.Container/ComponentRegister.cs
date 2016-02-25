using Core.Interfaces.Components;
using Core.Interfaces.Components.IoC;
using Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.IoC.Container
{
    public static class ComponentRegister
    {
        public static void Register(ComponentType types = ComponentType.All)
        {
            var components = TypeLocator.FindTypes("*.Component*.dll", typeof(ComponentBase));

            foreach(var component in components)
            {
                if (!component.IsAbstract) //skip abstract classes
                {
                    var atty = component.GetAttribute<ComponentAttribute>();

                    if (atty != null)
                    {
                        if (types.HasFlag(atty.Type) && !atty.DoNotRegister)
                        {
                            IoCContainer.Instance.Register(atty.InterfaceType, component, atty.ComponentLifeCycle);
                        }
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("Component of type \"{0}\" must have ComponentAttribute for auto bootstrapping.", component));
                    }
                }
            }
        }
    }
}
