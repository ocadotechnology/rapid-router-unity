using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using System.Linq;

namespace Zenject.Commands
{
    public abstract class CommandProviderBase<TCommand, TAction> : ProviderBase
        where TCommand : ICommand
    {
        public override Type GetInstanceType()
        {
            return typeof(TCommand);
        }

        public override object GetInstance(InjectContext context)
        {
            var obj = context.Container.Instantiate<TCommand>(GetCommandAction(context));
            Assert.That(obj != null);
            return obj;
        }

        protected abstract TAction GetCommandAction(InjectContext context);

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return context.Container.ValidateObjectGraph<TCommand>(context, typeof(TAction));
        }
    }
}

