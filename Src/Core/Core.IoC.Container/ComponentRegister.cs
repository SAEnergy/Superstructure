using Core.Interfaces.Components;
using Core.Interfaces.Components.IoC;
using Core.Util;
using System;

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
                        if ((types & atty.Type) == types && !atty.DoNotRegister)
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
