#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;
using UnityEngine;

namespace Zenject
{
    public class GameObjectSingletonLazyCreator
    {
        readonly DiContainer _container;
        readonly GameObjectSingletonProviderCreator _owner;
        readonly SingletonId _id;

        int _referenceCount;
        Component _component;

        public GameObjectSingletonLazyCreator(
            DiContainer container, GameObjectSingletonProviderCreator owner, SingletonId id)
        {
            Assert.That(id.ConcreteType.DerivesFrom<Component>());

            _container = container;
            _owner = owner;
            _id = id;
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
            if (_component == null)
            {
                var gameObj = new GameObject(_id.ConcreteIdentifier ?? _id.ConcreteType.Name());

                // Note that we always want to cache _container instead of using context.Container
                // since for singletons, the container they are accessed from should not determine
                // the container they are instantiated with
                // Transients can do that but not singletons

                gameObj.transform.SetParent(_container.DefaultParent, false);
                gameObj.SetActive(true);

                _component = gameObj.AddComponent(_id.ConcreteType);

                _container.Inject(_component, new object[0], true, context);
            }

            Assert.IsNotNull(_component);
            return _component;
        }

        public IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            // Note that we always want to cache _container instead of using context.Container
            // since for singletons, the container they are accessed from should not determine
            // the container they are instantiated with
            // Transients can do that but not singletons
            foreach (var err in _container.ValidateObjectGraph(_id.ConcreteType, context))
            {
                yield return err;
            }
        }
    }
}

#endif

