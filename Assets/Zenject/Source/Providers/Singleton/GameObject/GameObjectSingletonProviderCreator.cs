#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class GameObjectSingletonProviderCreator
    {
        readonly SingletonRegistry _singletonRegistry;
        readonly Dictionary<SingletonId, GameObjectSingletonLazyCreator> _creators = new Dictionary<SingletonId, GameObjectSingletonLazyCreator>();
        readonly DiContainer _container;

        public GameObjectSingletonProviderCreator(
            DiContainer container,
            SingletonRegistry singletonRegistry)
        {
            _singletonRegistry = singletonRegistry;
            _container = container;
        }

        public IEnumerable<GameObjectSingletonLazyCreator> Creators
        {
            get
            {
                return _creators.Values;
            }
        }

        public void RemoveCreator(SingletonId id)
        {
            bool success = _creators.Remove(id);
            Assert.That(success);
        }

        GameObjectSingletonLazyCreator AddCreator(SingletonId id)
        {
            GameObjectSingletonLazyCreator creator;

            if (!_creators.TryGetValue(id, out creator))
            {
                creator = new GameObjectSingletonLazyCreator(_container, this, id);
                _creators.Add(id, creator);
            }

            return creator;
        }

        public GameObjectSingletonProvider CreateProvider(
            Type concreteType, string concreteIdentifier)
        {
            Assert.That(concreteType.DerivesFrom<Component>());

            var singletonId = new SingletonId(concreteType, concreteIdentifier);
            var creator = AddCreator(singletonId);

            return new GameObjectSingletonProvider(creator, singletonId, _singletonRegistry);
        }
    }
}
#endif

