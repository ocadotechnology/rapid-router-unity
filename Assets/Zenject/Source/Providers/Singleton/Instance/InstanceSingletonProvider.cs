using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

namespace Zenject
{
    public class InstanceSingletonProvider : ProviderBase
    {
        readonly SingletonRegistry _singletonRegistry;
        readonly InstanceSingletonLazyCreator _lazyCreator;
        readonly SingletonId _id;

        public InstanceSingletonProvider(
            InstanceSingletonLazyCreator lazyCreator,
            SingletonRegistry singletonRegistry, SingletonId id)
        {
            _singletonRegistry = singletonRegistry;
            _lazyCreator = lazyCreator;
            _id = id;

            Init();
        }

        void Init()
        {
            _singletonRegistry.MarkSingleton(_id, SingletonTypes.ToSingleInstance);
            _lazyCreator.IncRefCount();
        }

        public override void Dispose()
        {
            _lazyCreator.DecRefCount();
            _singletonRegistry.UnmarkSingleton(_id, SingletonTypes.ToSingleInstance);
        }

        public override Type GetInstanceType()
        {
            return _id.ConcreteType;
        }

        public override object GetInstance(InjectContext context)
        {
            return _lazyCreator.Instance;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            // Nothing to validate
            yield break;
        }
    }
}

