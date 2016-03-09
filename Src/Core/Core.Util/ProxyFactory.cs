using Core.Interfaces.Components.IoC;
using System;
using System.Linq;
using System.Runtime.Remoting.Proxies;

namespace Core.Util
{
    public static class ProxyFactory
    {
        #region Public Methods

        public static object Wrap(object obj)
        {
            object retVal = obj;

            var proxyAtty = obj.GetType().GetAttribute<ProxyDecoratorAttribute>();

            if (proxyAtty != null)
            {
                retVal = Wrap(obj, proxyAtty.ProxyTypes);
            }

            return retVal;
        }

        public static object Wrap(object obj, params Type[] proxies)
        {
            object retVal = obj;

            if (proxies != null)
            {
                foreach (var proxy in proxies)
                {
                    if (typeof(RealProxy).IsAssignableFrom(proxy))
                    {
                        Type genericParamType = GetGenericParamaterType(obj.GetType());

                        if (genericParamType != null)
                        {
                            var generic = proxy.MakeGenericType(genericParamType);

                            if (generic != null)
                            {
                                var realProxy = Activator.CreateInstance(generic, new object[] { retVal }) as RealProxy;

                                if (realProxy != null)
                                {
                                    retVal = realProxy.GetTransparentProxy();
                                }
                                else
                                {
                                    throw new NullReferenceException(string.Format("ProxyFactory failed to create RealProxy instance of type \"{0}\".", proxy.FullName));
                                }
                            }
                            else
                            {
                                throw new NullReferenceException(string.Format("ProxyFactory failed to get proxy generic of type \"{0}\".", proxy.FullName));
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("Type \"{0}\" does not inherit from RealProxy.", proxy.FullName));
                    }
                }
            }

            return retVal;
        }

        #endregion

        #region Private Methods

        private static Type GetGenericParamaterType(Type objType)
        {
            Type type = null;

            //best option is the main object type, but only if it inherits from MarshalByRefObject
            if ((typeof(MarshalByRefObject).IsAssignableFrom(objType)))
            {
                type = objType;
            }
            else
            {
                //next see if there is a ComponentRegistrationAttribute
                var atty = objType.GetAttribute<ComponentRegistrationAttribute>();

                if (atty != null)
                {
                    type = atty.InterfaceType;
                }
                else
                {
                    //last try... find the interface it implements, this might not really work...
                    var allInterfaces = objType.GetInterfaces();

                    var minimalInterfaces = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).ToList(); //ignore inherited interfaces

                    if(minimalInterfaces.Count == 1)
                    {
                        type = minimalInterfaces.First();
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("Unable to find interface to proxy for object of type \"{0}\".", objType.FullName));
                    }
                }
            }

            if (type == null)
            {
                throw new NotSupportedException(string.Format("Object of type \"{0}\" must inherit from MarshalByRefObject or an interface, or have a ComponentRegistrationAttribute.", objType.FullName));
            }

            return type;
        }

        #endregion
    }
}
