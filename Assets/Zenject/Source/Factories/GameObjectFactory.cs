#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public abstract class GameObjectFactory : IValidatable
    {
        [Inject]
        protected readonly DiContainer _container;

        [Inject]
        protected readonly GameObject _prefab;

        [InjectOptional]
        string _groupName = null;

        public abstract IEnumerable<ZenjectResolveException> Validate();

        protected TValue CreateInternal<TValue>(List<TypeValuePair> argList)
        {
            return (TValue)_container.InstantiatePrefabForComponentExplicit(
                typeof(TValue), _prefab, argList,
                new InjectContext(_container, typeof(TValue), null), false, _groupName);
        }
    }

    public class GameObjectFactory<TValue> : GameObjectFactory, IFactory<TValue>
        // We don't want to do this to allow interfaces
        //where TValue : Component
    {
        public GameObjectFactory()
        {
            Assert.That(typeof(TValue).IsInterface() || typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create()
        {
            return CreateInternal<TValue>(new List<TypeValuePair>());
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>();
        }
    }

    // One parameter
    public class GameObjectFactory<TParam1, TValue> : GameObjectFactory, IFactory<TParam1, TValue>
        // We don't want to do this to allow interfaces
        //where TValue : Component
    {
        public GameObjectFactory()
        {
            Assert.That(typeof(TValue).IsInterface() || typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param)
        {
            return CreateInternal<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param),
                });
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1));
        }
    }

    // Two parameters
    public class GameObjectFactory<TParam1, TParam2, TValue> : GameObjectFactory, IFactory<TParam1, TParam2, TValue>
        // We don't want to do this to allow interfaces
        //where TValue : Component
    {
        public GameObjectFactory()
        {
            Assert.That(typeof(TValue).IsInterface() || typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param1, TParam2 param2)
        {
            return CreateInternal<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                });
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1), typeof(TParam2));
        }
    }

    // Three parameters
    public class GameObjectFactory<TParam1, TParam2, TParam3, TValue> : GameObjectFactory, IFactory<TParam1, TParam2, TParam3, TValue>
        // We don't want to do this to allow interfaces
        //where TValue : Component
    {
        public GameObjectFactory()
        {
            Assert.That(typeof(TValue).IsInterface() || typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return CreateInternal<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                });
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1), typeof(TParam2), typeof(TParam3));
        }
    }

    // Four parameters
    public class GameObjectFactory<TParam1, TParam2, TParam3, TParam4, TValue> : GameObjectFactory, IFactory<TParam1, TParam2, TParam3, TParam4, TValue>
        // We don't want to do this to allow interfaces
        //where TValue : Component
    {
        public GameObjectFactory()
        {
            Assert.That(typeof(TValue).IsInterface() || typeof(TValue).DerivesFrom<Component>());
        }

        public virtual TValue Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return CreateInternal<TValue>(
                new List<TypeValuePair>()
                {
                    InstantiateUtil.CreateTypePair(param1),
                    InstantiateUtil.CreateTypePair(param2),
                    InstantiateUtil.CreateTypePair(param3),
                    InstantiateUtil.CreateTypePair(param4),
                });
        }

        public override IEnumerable<ZenjectResolveException> Validate()
        {
            return _container.ValidateObjectGraph<TValue>(typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4));
        }
    }
}

#endif

