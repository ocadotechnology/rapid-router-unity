using System;
using System.Collections.Generic;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    // This class is just a facade that delegates requests to other singleton management classes
    public class SingletonProviderCreator
    {
        readonly MethodSingletonProviderCreator _methodSingletonProviderCreator;
        readonly TypeSingletonProviderCreator _typeSingletonProviderCreator;
        readonly InstanceSingletonProviderCreator _instanceSingletonProviderCreator;
        readonly FactorySingletonProviderCreator _factorySingletonProviderCreator;

#if !ZEN_NOT_UNITY3D
        readonly GameObjectSingletonProviderCreator _gameObjectSingletonProviderCreator;
        readonly PrefabSingletonProviderCreator _prefabSingletonProviderCreator;
        readonly PrefabResourceSingletonProviderCreator _prefabResourceSingletonProviderCreator;
        readonly MonoBehaviourSingletonProviderCreator _monoBehaviourSingletonProviderCreator;
#endif

        public SingletonProviderCreator(
            DiContainer container, SingletonRegistry singletonRegistry)
        {
            _typeSingletonProviderCreator = new TypeSingletonProviderCreator(container, singletonRegistry);
            _methodSingletonProviderCreator = new MethodSingletonProviderCreator(singletonRegistry);
            _instanceSingletonProviderCreator = new InstanceSingletonProviderCreator(container, singletonRegistry);
            _factorySingletonProviderCreator = new FactorySingletonProviderCreator(container, singletonRegistry);

#if !ZEN_NOT_UNITY3D
            _prefabResourceSingletonProviderCreator = new PrefabResourceSingletonProviderCreator(container, singletonRegistry);
            _prefabSingletonProviderCreator = new PrefabSingletonProviderCreator(container, singletonRegistry);
            _monoBehaviourSingletonProviderCreator = new MonoBehaviourSingletonProviderCreator(container, singletonRegistry);
            _gameObjectSingletonProviderCreator = new GameObjectSingletonProviderCreator(container, singletonRegistry);
#endif
        }

        public ProviderBase CreateProviderFromInstance(
            string concreteIdentifier, Type concreteType, object instance)
        {
            return CreateProviderFromInstance(
                new SingletonId(concreteType, concreteIdentifier), instance);
        }

        public ProviderBase CreateProviderFromInstance(
            SingletonId singleId, object instance)
        {
            return _instanceSingletonProviderCreator.CreateProvider(singleId, instance);
        }

        public ProviderBase CreateProviderFromMethod<TConcrete>(
            string concreteIdentifier, Func<InjectContext, TConcrete> method)
        {
            return _methodSingletonProviderCreator.CreateProvider<TConcrete>(concreteIdentifier, method);
        }

        public ProviderBase CreateProviderFromFactory<TContract, TFactory>(string concreteIdentifier)
            where TFactory : IFactory<TContract>
        {
            var id = new SingletonId(typeof(TContract), concreteIdentifier);
            return _factorySingletonProviderCreator.CreateProvider<TContract, TFactory>(id);
        }

        public ProviderBase CreateProviderFromType(SingletonId singleId)
        {
            return _typeSingletonProviderCreator.CreateProvider(singleId);
        }

        public ProviderBase CreateProviderFromType(string concreteIdentifier, Type concreteType)
        {
            return CreateProviderFromType(
                new SingletonId(concreteType, concreteIdentifier));
        }

#if !ZEN_NOT_UNITY3D
        public ProviderBase CreateProviderFromMonoBehaviour(
            string concreteIdentifier, Type concreteType, GameObject gameObject)
        {
            return _monoBehaviourSingletonProviderCreator.CreateProvider(
                concreteIdentifier, concreteType, gameObject);
        }

        public ProviderBase CreateProviderFromPrefabResource(
            string concreteIdentifier, Type concreteType, string resourcePath)
        {
            return _prefabResourceSingletonProviderCreator.CreateProvider(
                concreteIdentifier, concreteType, resourcePath);
        }

        public ProviderBase CreateProviderFromPrefab(
            string concreteIdentifier, Type concreteType, GameObject prefab)
        {
            return _prefabSingletonProviderCreator.CreateProvider(
                concreteIdentifier, concreteType, prefab);
        }

        public ProviderBase CreateProviderFromGameObject(
            Type concreteType, string concreteIdentifier)
        {
            return _gameObjectSingletonProviderCreator.CreateProvider(concreteType, concreteIdentifier);
        }
#endif
    }
}
