using System;
using System.Collections.Generic;

namespace Core.Interfaces.Components.IoC
{
    public interface IIoCContainer
    {
        List<KeyValuePair<Type, Type>> GetRegisteredTypes();

        void Register<TInterfaceType, TConcreteType>();

        void Register<TInterfaceType, TConcreteType>(LifeCycle lifeCycle);

        void Register(Type interfaceType, Type concreteType, LifeCycle lifeCycle);

        TInterfaceType Resolve<TInterfaceType>();

        object Resolve(Type interfaceType);
    }
}
