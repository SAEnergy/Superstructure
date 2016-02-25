using Core.Interfaces.Components.IoC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.IoC.Container
{
    public class IoCContainer : IIoCContainer
    {
        #region Fields

        #region Statics

        private static IoCContainer _instance;
        private static object _syncObject = new object();

        #endregion

        private Dictionary<Type, RegisteredObject> _registeredObjects;

        #endregion

        #region Properties

        public static IoCContainer Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(_syncObject)
                    {
                        if(_instance == null)
                        {
                            _instance = new IoCContainer();
                        }
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Constructor

        private IoCContainer()
        {
            _registeredObjects = new Dictionary<Type, RegisteredObject>();
        }

        #endregion

        #region Public Methods

        public List<KeyValuePair<Type, Type>> GetRegisteredTypes()
        {
            return _registeredObjects.Select(r => new KeyValuePair<Type, Type>(r.Key, r.Value.ConcreteType)).ToList();
        }

        public void Register<TInterfaceType, TConcreteType>()
        {
            Register<TInterfaceType, TConcreteType>(LifeCycle.Singleton);
        }

        public void Register<TInterfaceType, TConcreteType>(LifeCycle lifeCycle)
        {
            Register(typeof(TInterfaceType), typeof(TConcreteType), lifeCycle);
        }

        public void Register(Type interfaceType, Type concreteType, LifeCycle lifeCycle)
        {
            RegisteredObject obj = new RegisteredObject(interfaceType, concreteType, lifeCycle);

            _registeredObjects.Add(interfaceType, obj);
        }

        public object Resolve(Type interfaceType)
        {
            RegisteredObject obj = null;

            if (!_registeredObjects.TryGetValue(interfaceType, out obj))
            {
                throw new KeyNotFoundException(string.Format("The type \"{0}\" has not been registered.", interfaceType.Name));
            }

            return obj.GetInstance();
        }

        public TInterfaceType Resolve<TInterfaceType>()
        {
            return (TInterfaceType)Resolve(typeof(TInterfaceType));
        }

        #endregion
    }
}
