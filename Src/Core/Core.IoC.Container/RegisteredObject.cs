using Core.Interfaces.Base;
using Core.Interfaces.Components.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Proxies;

namespace Core.IoC.Container
{
    public class RegisteredObject
    {
        #region Fields

        private object _singleton;

        #endregion

        #region Properties

        public LifeCycle ObjectLifeCycle { get; private set; }

        public Type InterfaceType { get; private set; }

        public Type ConcreteType { get; private set; }

        public Type RealConcreteType { get; private set; }

        #endregion

        #region Constructor

        public RegisteredObject(Type interfaceType, Type concreteType, LifeCycle lifeCycle)
        {
            InterfaceType = interfaceType;
            ConcreteType = concreteType;
            RealConcreteType = GetRealConcreteType(concreteType);
            ObjectLifeCycle = lifeCycle;

            VerifyImplementation();
        }

        #endregion

        #region Public Methods

        public object GetInstance()
        {
            object obj = ObjectLifeCycle == LifeCycle.Singleton ? _singleton : null;

            if(ObjectLifeCycle == LifeCycle.Transient)
            {
                ConstructorInfo info = RealConcreteType.GetConstructors().First();
                obj = info.Invoke(BuildParameters(info));
            }
            else if(ObjectLifeCycle == LifeCycle.Singleton && obj == null)
            {
                MethodInfo info = RealConcreteType.GetMethod(SingletonBase.CREATEINSTANCEMETHODNAME, BindingFlags.Public | BindingFlags.Static);
                obj = info.Invoke(null, BuildParameters(info));

                if(obj == null)
                {
                    throw new NullReferenceException(string.Format("Singleton method \"{0}\" did not return an instance of type {1}.", SingletonBase.CREATEINSTANCEMETHODNAME, RealConcreteType.FullName));
                }

                _singleton = obj;
            }

            if (obj == null)
            {
                throw new NullReferenceException(string.Format("IoC container failed to create instance of type \"{0}\".", RealConcreteType.FullName));
            }

            if(typeof(RealProxy).IsAssignableFrom(ConcreteType))
            {
                var type = ConcreteType;

                while (typeof(RealProxy).IsAssignableFrom(type))
                {
                    var generic = type.GetGenericTypeDefinition().MakeGenericType(InterfaceType);

                    if (generic != null)
                    {
                        var proxy = Activator.CreateInstance(generic, new object[] { obj }) as RealProxy;

                        if (proxy != null)
                        {
                            obj = proxy.GetTransparentProxy();
                        }
                        else
                        {
                            throw new NullReferenceException(string.Format("IoC container failed to create proxy instance of type \"{0}\".", ConcreteType.FullName));
                        }
                    }
                    else
                    {
                        throw new NullReferenceException(string.Format("IoC container failed to get proxy generic of type \"{0}\".", ConcreteType.FullName));
                    }

                    type = type.GetGenericArguments().FirstOrDefault();
                }
            }

            return obj;
        }

        #endregion

        #region Private Methods

        private Type GetRealConcreteType(Type concreteType)
        {
            var type = concreteType;

            while(typeof(RealProxy).IsAssignableFrom(type))
            {
                type = type.GetGenericArguments().FirstOrDefault();
            }

            return type;
        }

        private void VerifyImplementation()
        {
            switch (ObjectLifeCycle)
            {
                case LifeCycle.Singleton:
                    VerifySingleton();
                    break;
                case LifeCycle.Transient:
                    VerifyTransient();
                    break;
                default:
                    throw new NotSupportedException(string.Format("LifeCycle \"{0}\" not yet supported.", ObjectLifeCycle.ToString()));
            }
        }

        private void VerifySingleton()
        {
            if(RealConcreteType.GetConstructors().Length > 0)
            {
                throw new NotSupportedException(string.Format("Singleton of type \"{0}\" cannot have a public constructor.", RealConcreteType.FullName));
            }

            if(RealConcreteType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Length != 1)
            {
                throw new NotSupportedException(string.Format("Singleton of type \"{0}\" must have one, and only one private constructor", RealConcreteType.FullName));
            }

            if (!CheckBaseType())
            {
                throw new NotSupportedException("IoC singletons must inherit from Singleton<T>");
            }
        }

        private void VerifyTransient()
        {
            if (RealConcreteType.GetConstructors().Length != 1)
            {
                throw new NotSupportedException("IoC concrete classes must have one, and only one public constructor.");
            }
        }

        private bool CheckBaseType()
        {
            bool rc = false;

            var baseType = RealConcreteType.BaseType;

            while(baseType != null)
            {
                if (baseType.IsGenericType)
                {
                    var genericType = baseType.GetGenericTypeDefinition();

                    if (genericType != null && genericType == typeof(Singleton<>))
                    {
                        if (baseType.GetGenericArguments().FirstOrDefault() == InterfaceType)
                        {
                            rc = true;
                            break;
                        }
                    }
                }

                baseType = baseType.BaseType;
            }

            return rc;
        }

        //this can get nasty, we cannot build parameters if there is a circular dependency
        //we will need to safe guard against this in the future if needed...
        private object[] BuildParameters(MethodBase info)
        {
            List<object> retVal = null;

            var parameterInfoes = info.GetParameters().ToList();

            if (parameterInfoes.Count > 0)
            {
                retVal = new List<object>();

                foreach (ParameterInfo parameterInfo in parameterInfoes)
                {
                    //you can only have other IoC objects as parameters
                    retVal.Add(IoCContainer.Instance.Resolve(parameterInfo.ParameterType));
                }
            }

            return retVal != null ? retVal.ToArray() : null;
        }

        #endregion
    }
}
