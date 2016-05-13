using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using System.Linq;

namespace Zenject.Commands
{
    public class CommandProviderStatic<TCommand, TAction> : ProviderBase
        where TCommand : ICommand
    {
        readonly Func<TAction> _methodGetter;

        public CommandProviderStatic(Func<TAction> methodGetter)
        {
            _methodGetter = methodGetter;
        }

        public override Type GetInstanceType()
        {
            return typeof(TCommand);
        }

        public override object GetInstance(InjectContext context)
        {
            var obj = context.Container.Instantiate<TCommand>(_methodGetter());
            Assert.That(obj != null);
            return obj;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return context.Container.ValidateObjectGraph<TCommand>(context, typeof(TAction));
        }
    }
}

