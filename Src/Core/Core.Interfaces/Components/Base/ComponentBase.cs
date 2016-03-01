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
        private List<ProxyAttribute> _proxyAttributes;

        protected ComponentBase()
        {
            //do not use util dll here to avoid requiring util reference across the application
            _componentRegistrationAttribute = GetType().GetCustomAttributes(typeof(ComponentRegistrationAttribute), true).FirstOrDefault() as ComponentRegistrationAttribute;

            _proxyAttributes = new List<ProxyAttribute>();

            foreach (var atty in GetType().GetCustomAttributes(typeof(ProxyAttribute), true))
            {
                var proxy = atty as ProxyAttribute;
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

        public List<Type> Proxies
        {
            get
            {
                List<Type> proxies = new List<Type>();

                _proxyAttributes.ForEach(p => proxies.Add(p.ProxyType));

                return proxies;
            }
        }
    }
}
