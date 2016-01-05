using Core.Interfaces.IoC;
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

            VerifyConstructor();
        }

        #endregion

        #region Public Methods

        public object GetInstance()
        {
            object obj = ObjectLifeCycle == LifeCycle.Singleton ? _singleton : null;

            if(obj == null || ObjectLifeCycle == LifeCycle.Transient)
            {
                ConstructorInfo info = ConcreteType.GetConstructors().First();
                obj = info.Invoke(BuildParameters(info));

                _singleton = ObjectLifeCycle == LifeCycle.Singleton ? obj : null; // do not keep a copy of transient objects!

            }

            return obj;
        }

        #endregion

        #region Private Methods

        private void VerifyConstructor()
        {
            if (ConcreteType.GetConstructors().Length != 1)
            {
                throw new Exception("IoC concrete classes can only have one constructor.");
            }
        }

        //this can get nasty, we cannot build parameters if there is a circular dependency
        //we will need to safe guard against this in the future if needed...
        private object[] BuildParameters(ConstructorInfo info)
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
