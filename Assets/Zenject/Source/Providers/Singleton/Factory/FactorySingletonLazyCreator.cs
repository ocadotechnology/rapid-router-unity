using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class FactorySingletonLazyCreator<TContract, TFactory> : IFactorySingletonLazyCreator
        where TFactory : IFactory<TContract>
    {
        readonly SingletonId _id;
        readonly FactorySingletonProviderCreator _owner;

        int _referenceCount;
        object _instance;
        DiContainer _container;

        public FactorySingletonLazyCreator(
            SingletonId id, DiContainer container,
            FactorySingletonProviderCreator owner)
        {
            _id = id;
            _container = container;
            _owner = owner;
        }

        public void IncRefCount()
        {
            _referenceCount += 1;
        }

        public void DecRefCount()
        {
            _referenceCount -= 1;

            if (_referenceCount <= 0)
            {
                _owner.RemoveCreator(_id);
            }
        }

        public object GetInstance(InjectContext context)
        {
            if (_instance != null)
            {
                return _instance;
            }

            // Note that we always want to cache _container instead of using context.Container
            // since for singletons, the container they are accessed from should not determine
            // the container they are instantiated with
            // Transients can do that but not singletons
            _instance = _container.Instantiate<TFactory>().Create();

            if (_instance == null)
            {
                throw new ZenjectResolveException(
                    "Failed to instantiate type '{0}' in FactorySingletonLazyCreator".Fmt(context.MemberType));
            }

            return _instance;
        }

        public IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            if (typeof(TFactory).DerivesFrom<IValidatable>())
            {
                var factory = _container.Instantiate<TFactory>(context);
                return ((IValidatable)factory).Validate();
            }

            return Enumerable.Empty<ZenjectResolveException>();
        }
    }
}


