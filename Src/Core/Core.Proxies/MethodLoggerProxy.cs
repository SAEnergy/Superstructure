using Core.Interfaces.Components.Logging;
using Core.IoC.Container;
using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Core.Proxies
{
    public class MethodLoggerProxy<T> : RealProxy
    {
        private readonly T _decorated;
        private readonly ILogger _logger;

        public MethodLoggerProxy(T decorated) : base(typeof(T))
        {
            _decorated = decorated;

            _logger = IoCContainer.Instance.Resolve<ILogger>();
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall.MethodBase as MethodInfo;
            _logger.Log(string.Format("In MethodLoggerProxy - Before executing \"{0}\".", methodCall.MethodName));

            try
            {
                var result = methodInfo.Invoke(_decorated, methodCall.InArgs);
                _logger.Log(string.Format("In MethodLoggerProxy - After executing \"{0}\".", methodCall.MethodName));

                return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception e)
            {
                _logger.Log(string.Format("In MethodLoggerProxy- Exception {0} executing \"{1}\".", e, methodCall.MethodName));

                return new ReturnMessage(e, methodCall);
            }
        }
    }
}
