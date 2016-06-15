using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

namespace Zenject.Commands
{
    public static class SignalExtensions
    {
        public static BindingConditionSetter BindSignal<TSignal>(this IBinder binder)
            where TSignal : ISignal
        {
            return binder.BindSignal<TSignal>(null);
        }

        public static BindingConditionSetter BindSignal<TSignal>(this IBinder binder, string identifier)
            where TSignal : ISignal
        {
            var container = (DiContainer)binder;
            return container.Bind<TSignal>(identifier).ToSingle(identifier);
        }

        public static BindingConditionSetter BindTrigger<TTrigger>(this IBinder binder)
            where TTrigger : ITrigger
        {
            return binder.BindTrigger<TTrigger>(null);
        }

        public static BindingConditionSetter BindTrigger<TTrigger>(this IBinder binder, string identifier)
            where TTrigger : ITrigger
        {
            var container = (DiContainer)binder;
            Type concreteSignalType = typeof(TTrigger).DeclaringType;

            Assert.IsNotNull(concreteSignalType);
            Assert.That(concreteSignalType.DerivesFrom<ISignal>());

            container.Bind(concreteSignalType.BaseType())
                .ToSingle(concreteSignalType, identifier)
                .When(ctx => ctx.ObjectType != null && ctx.ObjectType.DerivesFromOrEqual<TTrigger>() && ctx.ConcreteIdentifier == identifier);

            return container.Bind<TTrigger>(identifier).ToSingle(identifier);
        }
    }
}
