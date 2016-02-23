using Core.Interfaces.Components.Logging;
using Core.IoC.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;
using System.Reflection;
using System.Diagnostics;

namespace Core.Proxies
{
    public class MethodTimerProxy<T> : RealProxy
    {
        private readonly T _decorated;
        private readonly ILogger _logger;

        public MethodTimerProxy(T decorated) : base(typeof(T))
        {
            _decorated = decorated;

            _logger = IoCContainer.Instance.Resolve<ILogger>();
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall.MethodBase as MethodInfo;

            var sw = new Stopwatch();

            try
            {
                sw.Start();
                var result = methodInfo.Invoke(_decorated, methodCall.InArgs);

                return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
            }
            catch (Exception e)
            {
                return new ReturnMessage(e, methodCall);
            }
            finally
            {
                sw.Stop();

                _logger.Log(string.Format("Method {0} executed in: {1}ms", methodCall.MethodName, sw.ElapsedMilliseconds));
            }
        }
    }
}
