using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces.Components.IoC;
using System.Collections.ObjectModel;

namespace Core.Interfaces.Components
{
    public abstract class ComponentBase : IComponentBase
    {
        private ComponentAttribute _componetAttribute;
        private List<ProxyAttribute> _proxyAttributes;

        protected ComponentBase()
        {
            //do not use util dll here to avoid requiring util reference across the application
            _componetAttribute = GetType().GetCustomAttributes(typeof(ComponentAttribute), true).FirstOrDefault() as ComponentAttribute;

            _proxyAttributes = new List<ProxyAttribute>();

            foreach (var atty in GetType().GetCustomAttributes(typeof(ProxyAttribute), true))
            {
                var proxy = atty as ProxyAttribute;
                if(proxy != null)
                {
                    _proxyAttributes.Add(proxy);
                }
            }

            if(_componetAttribute == null)
            {
                throw new NotSupportedException(string.Format("Component of type \"{0}\" does not have a ComponentAttribute.", GetType()));
            }
        }

        public ComponentUserActions AllowedUserActions
        {
            get
            {
                return _componetAttribute.AllowedActions;
            }
        }

        public ComponentType ComponentType
        {
            get
            {
                return _componetAttribute.Type;
            }
        }

        public string Description
        {
            get
            {
                return _componetAttribute.Description;
            }
        }

        public string FriendName
        {
            get
            {
                return _componetAttribute.FriendlyName;
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
