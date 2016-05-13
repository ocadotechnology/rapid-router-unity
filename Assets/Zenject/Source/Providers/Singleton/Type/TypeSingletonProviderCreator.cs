using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    //[System.Diagnostics.DebuggerStepThrough]
    public class TypeSingletonProviderCreator
    {
        readonly Dictionary<SingletonId, TypeSingletonLazyCreator> _creators = new Dictionary<SingletonId, TypeSingletonLazyCreator>();
        readonly DiContainer _container;
        readonly SingletonRegistry _singletonRegistry;

        public TypeSingletonProviderCreator(
            DiContainer container, SingletonRegistry singletonRegistry)
        {
            _container = container;
            _singletonRegistry = singletonRegistry;
        }

        public void RemoveCreator(SingletonId id)
        {
            bool success = _creators.Remove(id);
            Assert.That(success);
        }

        TypeSingletonLazyCreator AddCreator(SingletonId id)
        {
            TypeSingletonLazyCreator creator;

            if (!_creators.TryGetValue(id, out creator))
            {
                creator = new TypeSingletonLazyCreator(id, this, _container);
                _creators.Add(id, creator);
            }

            return creator;
        }

        public TypeSingletonProvider CreateProvider(SingletonId singletonId)
        {
            var creator = AddCreator(singletonId);
            return new TypeSingletonProvider(
                creator, singletonId, _singletonRegistry);
        }
    }
}
