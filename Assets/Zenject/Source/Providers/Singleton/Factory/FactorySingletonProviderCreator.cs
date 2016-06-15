using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    //[System.Diagnostics.DebuggerStepThrough]
    public class FactorySingletonProviderCreator
    {
        readonly SingletonRegistry _singletonRegistry;
        readonly Dictionary<SingletonId, IFactorySingletonLazyCreator> _creators = new Dictionary<SingletonId, IFactorySingletonLazyCreator>();
        readonly DiContainer _container;

        public FactorySingletonProviderCreator(
            DiContainer container,
            SingletonRegistry singletonRegistry)
        {
            _singletonRegistry = singletonRegistry;
            _container = container;
        }

        public void RemoveCreator(SingletonId id)
        {
            bool success = _creators.Remove(id);
            Assert.That(success);
        }

        FactorySingletonLazyCreator<TContract, TFactory> AddCreator<TContract, TFactory>(SingletonId id)
            where TFactory : IFactory<TContract>
        {
            IFactorySingletonLazyCreator creator;

            if (!_creators.TryGetValue(id, out creator))
            {
                creator = new FactorySingletonLazyCreator
                    <TContract, TFactory>(id, _container, this);
                _creators.Add(id, creator);
            }

            return (FactorySingletonLazyCreator<TContract, TFactory>)creator;
        }

        public FactorySingletonProvider CreateProvider<TContract, TFactory>(SingletonId singletonId)
            where TFactory : IFactory<TContract>
        {
            Assert.IsEqual(typeof(TContract), singletonId.ConcreteType);

            var creator = AddCreator<TContract, TFactory>(singletonId);

            return new FactorySingletonProvider(creator, _singletonRegistry, singletonId);
        }
    }
}
