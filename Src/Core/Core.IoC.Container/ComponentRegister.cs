using Core.Interfaces.Components.Base;
using Core.Interfaces.Components.IoC;
using Core.Util;

namespace Core.IoC.Container
{
    public static class ComponentRegister
    {
        public static void Register(ComponentType types = ComponentType.All)
        {
            var components = TypeLocator.FindTypes("*Component*.dll", typeof(ComponentBase));

            foreach(var component in components)
            {
                if (!component.IsAbstract) //skip abstract classes
                {
                    var atty = component.GetAttribute<ComponentRegistrationAttribute>();

                    if (atty != null)
                    {
                        if ((types & atty.Type) == types && !atty.DoNotRegister)
                        {
                            var lifeCycle = typeof(SingletonBase).IsAssignableFrom(component) ? LifeCycle.Singleton : LifeCycle.Transient;

                            IoCContainer.Instance.Register(atty.InterfaceType, component, lifeCycle);
                        }
                    }
                }
            }
        }
    }
}
