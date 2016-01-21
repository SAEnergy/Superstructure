using System;
using System.Collections.Generic;

namespace Core.Interfaces.Base
{
    public abstract class Singleton<T> : SingletonBase where T : class
    {
        #region Fields

        private static T _singleton = default(T);
        private static object _singletonSyncObject = new object();

        #endregion

        #region Properties

        public static T Instance
        {
            get
            {
                if(EqualityComparer<T>.Default.Equals(_singleton, default(T)))
                {
                    throw new NullReferenceException(string.Format("Singleton has not been created.  Must call \"{0}\" with required dependencies.", CREATEINSTANCEMETHODNAME));
                }

                return _singleton;
            }

            protected set
            {
                if(EqualityComparer<T>.Default.Equals(_singleton, default(T)))
                {
                    lock(_singletonSyncObject)
                    {
                        if (EqualityComparer<T>.Default.Equals(_singleton, default(T)))
                        {
                            _singleton = value;
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException("Cannot set singleton instance more than once.");
                }
            }
        }

        #endregion

        #region Constructor

        protected Singleton()
        {
            VerfiySingletonPattern();
        }

        #endregion

        #region Private and Protected Methods

        private void VerfiySingletonPattern()
        {
            if (!typeof(T).IsInterface)
            {
                throw new NotSupportedException("The base singleton generic type must be an interface type.");
            }

            var info = GetCreateInstanceMethod();

            if (info != null && info.ReturnType != typeof(T))
            {
                throw new NotSupportedException(string.Format("The base singleton \"{0}\" must have a return type that matches the generic parameter of type \"{1}\"", CREATEINSTANCEMETHODNAME, typeof(T).FullName));
            }
        }

        #endregion
    }
}
