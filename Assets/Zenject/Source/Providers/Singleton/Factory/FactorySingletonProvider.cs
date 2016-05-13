using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

namespace Zenject
{
    public class FactorySingletonProvider : ProviderBase
    {
        readonly SingletonId _id;
        readonly SingletonRegistry _singletonRegistry;
        readonly IFactorySingletonLazyCreator _lazyCreator;

        public FactorySingletonProvider(
            IFactorySingletonLazyCreator lazyCreator,
            SingletonRegistry singletonRegistry,
            SingletonId id)
        {
            _id = id;
            _singletonRegistry = singletonRegistry;
            _lazyCreator = lazyCreator;

            Init();
        }

        void Init()
        {
            _singletonRegistry.MarkSingleton(_id, SingletonTypes.ToSingleFactory);
            _lazyCreator.IncRefCount();
        }

        public override void Dispose()
        {
            _lazyCreator.DecRefCount();
            _singletonRegistry.UnmarkSingleton(_id, SingletonTypes.ToSingleFactory);
        }

        public override Type GetInstanceType()
        {
            return _id.ConcreteType;
        }

        public override object GetInstance(InjectContext context)
        {
            return _lazyCreator.GetInstance(context);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return _lazyCreator.ValidateBinding(context);
        }
    }
}

