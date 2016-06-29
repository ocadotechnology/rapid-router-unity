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
    internal class MonoBehaviourSingletonProvider : ProviderBase
    {
        readonly MonoBehaviourSingletonProviderCreator _owner;
        readonly SingletonRegistry _singletonRegistry;
        readonly MonoBehaviourSingletonLazyCreator _lazyCreator;
        readonly Type _componentType;
        readonly MonoBehaviourSingletonId _monoBehaviourId;
        readonly SingletonId _singletonId;

        public MonoBehaviourSingletonProvider(
            MonoBehaviourSingletonId monoBehaviourId,
            Type componentType,
            MonoBehaviourSingletonLazyCreator lazyCreator,
            SingletonRegistry singletonRegistry,
            MonoBehaviourSingletonProviderCreator owner)
        {
            _owner = owner;
            Assert.That(componentType.DerivesFromOrEqual<Component>());

            _singletonRegistry = singletonRegistry;
            _lazyCreator = lazyCreator;
            _componentType = componentType;
            _monoBehaviourId = monoBehaviourId;
            _singletonId = new SingletonId(componentType, monoBehaviourId.ConcreteIdentifier);

            Init();
        }

        void Init()
        {
            _owner.MarkGameObject(_singletonId, _monoBehaviourId.GameObject);
            _singletonRegistry.MarkSingleton(_singletonId, SingletonTypes.ToSingleMonoBehaviour);
            _lazyCreator.IncRefCount();
        }

        public override void Dispose()
        {
            _lazyCreator.DecRefCount();
            _singletonRegistry.UnmarkSingleton(_singletonId, SingletonTypes.ToSingleMonoBehaviour);
            _owner.UnmarkGameObject(_singletonId, _monoBehaviourId.GameObject);
        }

        public override Type GetInstanceType()
        {
            return _componentType;
        }

        public override object GetInstance(InjectContext context)
        {
            return _lazyCreator.GetInstance(context);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return _lazyCreator.ValidateBinding(context);
        }
    }
}

#endif
