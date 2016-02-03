using Core.Interfaces.Base;
using Core.Interfaces.Components.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        #endregion

        #region Constructor

        public RegisteredObject(Type interfaceType, Type concreteType, LifeCycle lifeCycle)
        {
            InterfaceType = interfaceType;
            ConcreteType = concreteType;
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
                ConstructorInfo info = ConcreteType.GetConstructors().First();
                obj = info.Invoke(BuildParameters(info));
            }
            else if(ObjectLifeCycle == LifeCycle.Singleton && obj == null)
            {
                MethodInfo info = ConcreteType.GetMethod(SingletonBase.CREATEINSTANCEMETHODNAME, BindingFlags.Public | BindingFlags.Static);
                obj = info.Invoke(null, BuildParameters(info));

                if(obj == null)
                {
                    throw new NullReferenceException(string.Format("Singleton method \"{0}\" did not return an instance of type {1}.", SingletonBase.CREATEINSTANCEMETHODNAME, ConcreteType.FullName));
                }

                _singleton = obj;
            }

            if (obj == null)
            {
                throw new NullReferenceException(string.Format("IoC container Failed to create instance of type \"{0}\".", ConcreteType.FullName));
            }

            return obj;
        }

        #endregion

        #region Private Methods

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
            if(ConcreteType.GetConstructors().Length > 0)
            {
                throw new NotSupportedException(string.Format("Singleton of type \"{0}\" cannot have a public constructor.", ConcreteType.FullName));
            }

            if(ConcreteType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Length != 1)
            {
                throw new NotSupportedException(string.Format("Singleton of type \"{0}\" must have one, and only one private constructor", ConcreteType.FullName));
            }

            if (!CheckBaseType())
            {
                throw new NotSupportedException("IoC singletons must inherit from Singleton<T>");
            }
        }

        private void VerifyTransient()
        {
            if (ConcreteType.GetConstructors().Length != 1)
            {
                throw new NotSupportedException("IoC concrete classes must have one, and only one public constructor.");
            }
        }

        private bool CheckBaseType()
        {
            bool rc = false;

            var baseType = ConcreteType.BaseType;

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
