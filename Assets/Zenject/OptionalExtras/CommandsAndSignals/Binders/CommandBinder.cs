using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ModestTree;
using ModestTree.Util;
using System.Linq;

namespace Zenject.Commands
{
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinderBase<TCommand, TAction> : BinderBase
        where TCommand : ICommand
    {
        readonly SingletonProviderCreator _singletonProviderFactory;

        public CommandBinderBase(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, typeof(TCommand), identifier)
        {
            _singletonProviderFactory = singletonProviderFactory;
        }

        public BindingConditionSetter HandleWithStaticMethod(Func<TAction> methodGetter)
        {
            return ToProvider(new CommandProviderStatic<TCommand, TAction>(methodGetter));
        }

        protected ProviderBase CreateSingletonProvider<THandler>(string concreteIdentifier)
        {
            return _singletonProviderFactory.CreateProviderFromType(concreteIdentifier, typeof(THandler));
        }
    }

    // Zero parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand> : CommandBinderBase<TCommand, Action>
        where TCommand : Command
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter HandleWithTransient<THandler>(
            Func<THandler, Action> methodGetter)
        {
            return ToProvider(
                new CommandProviderMethodTransient<TCommand, THandler>(
                    methodGetter));
        }

        public BindingConditionSetter HandleWithTransient<THandler>()
            where THandler : ICommandHandler
        {
            return ToProvider(
                new CommandProviderHandlerTransient<TCommand, THandler>());
        }

        public BindingConditionSetter HandleWithSingle<THandler>()
            where THandler : ICommandHandler
        {
            return HandleWithSingle<THandler>((string)null);
        }

        public BindingConditionSetter HandleWithSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler
        {
            return ToProvider(
                new CommandProviderHandlerSingle<TCommand, THandler>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(
            string concreteIdentifier, Func<THandler, Action> methodGetter)
        {
            return ToProvider(new CommandProviderMethodSingle<TCommand, THandler>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(Func<THandler, Action> methodGetter)
        {
            return HandleWithSingle<THandler>(null, methodGetter);
        }
    }

    // One parameter
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1> : CommandBinderBase<TCommand, Action<TParam1>>
        where TCommand : Command<TParam1>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter HandleWithTransient<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1>> methodGetter)
        {
            return ToProvider(
                new CommandProviderMethodTransient<TCommand, THandler, TParam1>(
                    methodGetter));
        }

        public BindingConditionSetter HandleWithTransient<THandler>(Func<THandler, Action<TParam1>> methodGetter)
        {
            return HandleWithTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter HandleWithSingle<THandler>()
            where THandler : ICommandHandler<TParam1>
        {
            return HandleWithSingle<THandler>((string)null);
        }

        public BindingConditionSetter HandleWithSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1>
        {
            return ToProvider(
                new CommandProviderHandlerSingle<TCommand, THandler, TParam1>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1>> methodGetter)
        {
            return ToProvider(new CommandProviderMethodSingle<TCommand, THandler, TParam1>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(Func<THandler, Action<TParam1>> methodGetter)
        {
            return HandleWithSingle<THandler>(null, methodGetter);
        }
    }

    // Two parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1, TParam2> : CommandBinderBase<TCommand, Action<TParam1, TParam2>>
        where TCommand : Command<TParam1, TParam2>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter HandleWithTransient<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return ToProvider(
                new CommandProviderMethodTransient<TCommand, THandler, TParam1, TParam2>(
                    methodGetter));
        }

        public BindingConditionSetter HandleWithTransient<THandler>(Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return HandleWithTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter HandleWithSingle<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2>
        {
            return HandleWithSingle<THandler>((string)null);
        }

        public BindingConditionSetter HandleWithSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1, TParam2>
        {
            return ToProvider(
                new CommandProviderHandlerSingle<TCommand, THandler, TParam1, TParam2>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return ToProvider(new CommandProviderMethodSingle<TCommand, THandler, TParam1, TParam2>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(Func<THandler, Action<TParam1, TParam2>> methodGetter)
        {
            return HandleWithSingle<THandler>(null, methodGetter);
        }
    }

    // Three parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1, TParam2, TParam3> : CommandBinderBase<TCommand, Action<TParam1, TParam2, TParam3>>
        where TCommand : Command<TParam1, TParam2, TParam3>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter HandleWithTransient<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return ToProvider(
                new CommandProviderMethodTransient<TCommand, THandler, TParam1, TParam2, TParam3>(
                    methodGetter));
        }

        public BindingConditionSetter HandleWithTransient<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return HandleWithTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter HandleWithSingle<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3>
        {
            return HandleWithSingle<THandler>((string)null);
        }

        public BindingConditionSetter HandleWithSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3>
        {
            return ToProvider(
                new CommandProviderHandlerSingle<TCommand, THandler, TParam1, TParam2, TParam3>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return ToProvider(new CommandProviderMethodSingle<TCommand, THandler, TParam1, TParam2, TParam3>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3>> methodGetter)
        {
            return HandleWithSingle<THandler>(null, methodGetter);
        }
    }

    // Four parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4> : CommandBinderBase<TCommand, Action<TParam1, TParam2, TParam3, TParam4>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter HandleWithTransient<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return ToProvider(
                new CommandProviderMethodTransient<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(
                    methodGetter));
        }

        public BindingConditionSetter HandleWithTransient<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return HandleWithTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter HandleWithSingle<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4>
        {
            return HandleWithSingle<THandler>((string)null);
        }

        public BindingConditionSetter HandleWithSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4>
        {
            return ToProvider(
                new CommandProviderHandlerSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(
            string concreteIdentifier, Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return ToProvider(new CommandProviderMethodSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(Func<THandler, Action<TParam1, TParam2, TParam3, TParam4>> methodGetter)
        {
            return HandleWithSingle<THandler>(null, methodGetter);
        }
    }

    // Five parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5> : CommandBinderBase<TCommand, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter HandleWithTransient<THandler>(
            string concreteIdentifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return ToProvider(
                new CommandProviderMethodTransient<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(
                    methodGetter));
        }

        public BindingConditionSetter HandleWithTransient<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return HandleWithTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter HandleWithSingle<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return HandleWithSingle<THandler>((string)null);
        }

        public BindingConditionSetter HandleWithSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5>
        {
            return ToProvider(
                new CommandProviderHandlerSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(
            string concreteIdentifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return ToProvider(new CommandProviderMethodSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5>> methodGetter)
        {
            return HandleWithSingle<THandler>(null, methodGetter);
        }
    }

    // Six parameters
    [System.Diagnostics.DebuggerStepThrough]
    public class CommandBinder<TCommand, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> : CommandBinderBase<TCommand, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>>
        where TCommand : Command<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
    {
        public CommandBinder(
            DiContainer container, string identifier, SingletonProviderCreator singletonProviderFactory)
            : base(container, identifier, singletonProviderFactory)
        {
        }

        public BindingConditionSetter HandleWithTransient<THandler>(
            string concreteIdentifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return ToProvider(
                new CommandProviderMethodTransient<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
                    methodGetter));
        }

        public BindingConditionSetter HandleWithTransient<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return HandleWithTransient<THandler>(null, methodGetter);
        }

        public BindingConditionSetter HandleWithSingle<THandler>()
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return HandleWithSingle<THandler>((string)null);
        }

        public BindingConditionSetter HandleWithSingle<THandler>(string concreteIdentifier)
            where THandler : ICommandHandler<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        {
            return ToProvider(
                new CommandProviderHandlerSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
                    CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(
            string concreteIdentifier, Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return ToProvider(new CommandProviderMethodSingle<TCommand, THandler, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(
                methodGetter, CreateSingletonProvider<THandler>(concreteIdentifier)));
        }

        public BindingConditionSetter HandleWithSingle<THandler>(Func<THandler, ModestTree.Util.Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>> methodGetter)
        {
            return HandleWithSingle<THandler>(null, methodGetter);
        }
    }

}
