using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

namespace Zenject
{
    public class MethodSingletonProvider : ProviderBase
    {
        readonly SingletonRegistry _singletonRegistry;
        readonly SingletonId _id;
        readonly IMethodSingletonLazyCreator _lazyCreator;

        public MethodSingletonProvider(
            IMethodSingletonLazyCreator lazyCreator,
            SingletonId id,
            SingletonRegistry singletonRegistry)
        {
            _singletonRegistry = singletonRegistry;
            _id = id;
            _lazyCreator = lazyCreator;

            Init();
        }

        void Init()
        {
            _singletonRegistry.MarkSingleton(_id, SingletonTypes.ToSingleMethod);
            _lazyCreator.IncRefCount();
        }

        public override void Dispose()
        {
            _lazyCreator.DecRefCount();
            _singletonRegistry.UnmarkSingleton(_id, SingletonTypes.ToSingleMethod);
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
            // Nothing we can really validate here
            yield break;
        }
    }
}

