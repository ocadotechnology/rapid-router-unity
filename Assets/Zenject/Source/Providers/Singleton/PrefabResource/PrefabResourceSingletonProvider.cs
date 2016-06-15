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
    public class PrefabResourceSingletonProvider : ProviderBase
    {
        readonly PrefabResourceSingletonProviderCreator _owner;
        readonly SingletonRegistry _singletonRegistry;
        readonly PrefabResourceSingletonLazyCreator _lazyCreator;
        readonly Type _componentType;
        readonly PrefabResourceSingletonId _resourceId;
        readonly SingletonId _singletonId;

        public PrefabResourceSingletonProvider(
            PrefabResourceSingletonId resourceId, Type componentType,
            PrefabResourceSingletonLazyCreator lazyCreator,
            SingletonRegistry singletonRegistry,
            PrefabResourceSingletonProviderCreator owner)
        {
            _owner = owner;
            Assert.That(componentType.DerivesFromOrEqual<Component>());

            _singletonRegistry = singletonRegistry;
            _lazyCreator = lazyCreator;
            _componentType = componentType;
            _resourceId = resourceId;
            _singletonId = new SingletonId(componentType, resourceId.ConcreteIdentifier);

            Init();
        }

        void Init()
        {
            _owner.MarkResource(_singletonId, _resourceId.ResourcePath);
            _singletonRegistry.MarkSingleton(_singletonId, SingletonTypes.ToSinglePrefabResource);
            _lazyCreator.IncRefCount();
        }

        public override void Dispose()
        {
            _lazyCreator.DecRefCount();
            _singletonRegistry.UnmarkSingleton(_singletonId, SingletonTypes.ToSinglePrefabResource);
            _owner.UnmarkResource(_singletonId, _resourceId.ResourcePath);
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
