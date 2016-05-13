#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;
using UnityEngine;

namespace Zenject
{
    public class MonoBehaviourSingletonLazyCreator
    {
        readonly DiContainer _container;
        readonly MonoBehaviourSingletonProviderCreator _owner;
        readonly MonoBehaviourSingletonId _id;

        Component _instance;
        int _referenceCount;

        public MonoBehaviourSingletonLazyCreator(
            DiContainer container, MonoBehaviourSingletonProviderCreator owner,
            MonoBehaviourSingletonId id)
        {
            Assert.That(id.ConcreteType.DerivesFromOrEqual<Component>());

            _container = container;
            _owner = owner;
            _id = id;
        }

        GameObject GameObject
        {
            get
            {
                return _id.GameObject;
            }
        }

        Type ComponentType
        {
            get
            {
                return _id.ConcreteType;
            }
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
            Assert.That(ComponentType.DerivesFromOrEqual(context.MemberType));

            if (_instance == null)
            {
                Assert.That(!_container.IsValidating,
                    "Tried to instantiate a MonoBehaviour with type '{0}' during validation. Object graph: {1}", ComponentType, context.GetObjectGraphString());

                // Note that we always want to cache _container instead of using context.Container
                // since for singletons, the container they are accessed from should not determine
                // the container they are instantiated with
                // Transients can do that but not singletons

                _instance = GameObject.AddComponent(ComponentType);
                Assert.That(_instance != null);

                _container.Inject(_instance, new object[0], true, context);
            }

            return _instance;
        }

        public IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return _container.ValidateObjectGraph(ComponentType, context);
        }
    }
}

#endif
