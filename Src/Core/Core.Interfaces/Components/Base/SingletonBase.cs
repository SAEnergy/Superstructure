using System;
using System.Reflection;

namespace Core.Interfaces.Components.Base
{
    public abstract class SingletonBase : ComponentBase
    {
        #region Fields

        public const string CREATEINSTANCEMETHODNAME = "CreateInstance";

        #endregion

        #region Constructor

        protected SingletonBase()
        {
            ValidateSingletonBasePattern();
        }

        #endregion

        #region Private Methods

        private void ValidateSingletonBasePattern()
        {
            if (GetCreateInstanceMethod() == null)
            {
                throw new NotSupportedException(string.Format("The singleton class must implement a public static method by the name of \"{0}\".", CREATEINSTANCEMETHODNAME));
            }
        }

        protected MethodInfo GetCreateInstanceMethod()
        {
            return GetType().GetMethod(CREATEINSTANCEMETHODNAME, BindingFlags.Public | BindingFlags.Static);
        }

        #endregion

    }
}
