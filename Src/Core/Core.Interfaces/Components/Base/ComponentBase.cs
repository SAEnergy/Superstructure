using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces.Components.IoC;
using Core.Models;

namespace Core.Interfaces.Components.Base
{
    public abstract class ComponentBase : IComponentBase
    {
        private ComponentRegistrationAttribute _componentRegistrationAttribute;
        private List<ProxyDecoratorAttribute> _proxyAttributes;

        protected ComponentBase()
        {
            //do not use util dll here to avoid requiring util reference across the application
            _componentRegistrationAttribute = GetType().GetCustomAttributes(typeof(ComponentRegistrationAttribute), true).FirstOrDefault() as ComponentRegistrationAttribute;

            _proxyAttributes = new List<ProxyDecoratorAttribute>();

            foreach (var atty in GetType().GetCustomAttributes(typeof(ProxyDecoratorAttribute), true))
            {
                var proxy = atty as ProxyDecoratorAttribute;
                if(proxy != null)
                {
                    _proxyAttributes.Add(proxy);
                }
            }
        }

        public ComponentType ComponentType
        {
            get
            {
                return _componentRegistrationAttribute != null ? _componentRegistrationAttribute.Type : ComponentType.NotConfigured;
            }
        }
    }
}
