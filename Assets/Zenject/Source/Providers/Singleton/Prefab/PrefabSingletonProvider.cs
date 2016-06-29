#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    // NOTE: we need the provider seperate from the lazyCreator because
    // if we return the same provider multiple times then the condition
    // will get over-written
    public class PrefabSingletonProvider : ProviderBase
    {
        readonly PrefabSingletonProviderCreator _owner;
        readonly SingletonRegistry _singletonRegistry;
        readonly PrefabSingletonLazyCreator _lazyCreator;
        readonly Type _componentType;
        readonly PrefabSingletonId _prefabId;
        readonly SingletonId _singletonId;

        public PrefabSingletonProvider(
            PrefabSingletonId prefabId, Type componentType,
            PrefabSingletonLazyCreator lazyCreator,
            SingletonRegistry singletonRegistry,
            PrefabSingletonProviderCreator owner)
        {
            _owner = owner;
            Assert.That(componentType.DerivesFromOrEqual<Component>());

            _singletonRegistry = singletonRegistry;
            _lazyCreator = lazyCreator;
            _componentType = componentType;
            _prefabId = prefabId;
            _singletonId = new SingletonId(componentType, prefabId.ConcreteIdentifier);

            Init();
        }

        void Init()
        {
            _owner.MarkPrefab(_singletonId, _prefabId.Prefab);
            _singletonRegistry.MarkSingleton(_singletonId, SingletonTypes.ToSinglePrefab);
            _lazyCreator.IncRefCount();
        }

        public override void Dispose()
        {
            _lazyCreator.DecRefCount();
            _singletonRegistry.UnmarkSingleton(_singletonId, SingletonTypes.ToSinglePrefab);
            _owner.UnmarkPrefab(_singletonId, _prefabId.Prefab);
        }

        public override Type GetInstanceType()
        {
            return _componentType;
        }

        public override object GetInstance(InjectContext context)
        {
            return _lazyCreator.GetComponent(_componentType, context);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return _lazyCreator.ValidateBinding(_componentType, context);
        }
    }
}

#endif
