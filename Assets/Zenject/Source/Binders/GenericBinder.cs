using System;
using ModestTree;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    // All this class does is provide generic constraints on all the inherited bind methods
    public class GenericBinder<TContract> : TypeBinder
    {
        public GenericBinder(
            DiContainer container, string identifier)
            : base(container, typeof(TContract), identifier)
        {
        }

        public BindingConditionSetter ToLookup<TConcrete>()
            where TConcrete : TContract
        {
            return ToLookupBase<TConcrete>(null);
        }

        public BindingConditionSetter ToLookup<TConcrete>(string identifier)
            where TConcrete : TContract
        {
            return ToLookupBase<TConcrete>(identifier);
        }

        public BindingConditionSetter ToMethod(Func<InjectContext, TContract> method)
        {
            return ToMethodBase<TContract>(method);
        }

        public BindingConditionSetter ToGetter<TObj>(Func<TObj, TContract> method)
        {
            return ToGetterBase<TObj, TContract>(null, method);
        }

        public BindingConditionSetter ToGetter<TObj>(string identifier, Func<TObj, TContract> method)
        {
            return ToGetterBase<TObj, TContract>(identifier, method);
        }

        public BindingConditionSetter ToTransient<TConcrete>()
            where TConcrete : TContract
        {
            return ToTransient(typeof(TConcrete));
        }

        public BindingConditionSetter ToSingle<TConcrete>(string concreteIdentifier)
            where TConcrete : TContract
        {
            return ToSingle(typeof(TConcrete), concreteIdentifier);
        }

        public BindingConditionSetter ToInstance<TConcrete>(TConcrete instance)
            where TConcrete : TContract
        {
            return ToInstance(instance == null ? typeof(TConcrete) : instance.GetType(), instance);
        }

        public BindingConditionSetter ToSingleInstance<TConcrete>(TConcrete instance)
            where TConcrete : TContract
        {
            return ToSingleInstance(instance == null ? typeof(TConcrete) : instance.GetType(), null, instance);
        }

        public BindingConditionSetter ToSingleInstance<TConcrete>(string concreteIdentifier, TConcrete instance)
            where TConcrete : TContract
        {
            return ToSingleInstance(instance == null ? typeof(TConcrete) : instance.GetType(), concreteIdentifier, instance);
        }

        public BindingConditionSetter ToSingleMethod<TConcrete>(string concreteIdentifier, Func<InjectContext, TConcrete> method)
            where TConcrete : TContract
        {
            return ToSingleMethodBase<TConcrete>(concreteIdentifier, method);
        }

        public BindingConditionSetter ToSingleMethod<TConcrete>(Func<InjectContext, TConcrete> method)
            where TConcrete : TContract
        {
            return ToSingleMethodBase<TConcrete>(null, method);
        }

        public BindingConditionSetter ToSingleFactory<TFactory>()
            where TFactory : IFactory<TContract>
        {
            return ToSingleFactoryBase<TContract, TFactory>(null);
        }

        public BindingConditionSetter ToSingleFactory<TFactory>(string concreteIdentifier)
            where TFactory : IFactory<TContract>
        {
            return ToSingleFactoryBase<TContract, TFactory>(concreteIdentifier);
        }

        public BindingConditionSetter ToSingleFactory<TFactory, TConcrete>()
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract
        {
            return ToSingleFactoryBase<TConcrete, TFactory>(null);
        }

        public BindingConditionSetter ToSingleFactory<TFactory, TConcrete>(string concreteIdentifier)
            where TFactory : IFactory<TConcrete>
            where TConcrete : TContract
        {
            return ToSingleFactoryBase<TConcrete, TFactory>(concreteIdentifier);
        }

        public BindingConditionSetter ToSingle<TConcrete>()
            where TConcrete : TContract
        {
            return ToSingle(typeof(TConcrete), null);
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(GameObject gameObject)
            where TConcrete : TContract
        {
            return ToSingleMonoBehaviour<TConcrete>(null, gameObject);
        }

        public BindingConditionSetter ToSingleMonoBehaviour<TConcrete>(string concreteIdentifier, GameObject gameObject)
            where TConcrete : TContract
        {
            return ToSingleMonoBehaviour(concreteIdentifier, typeof(TConcrete), gameObject);
        }

        public BindingConditionSetter ToResource<TConcrete>(string resourcePath)
            where TConcrete : TContract
        {
            return ToResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientPrefabResource<TConcrete>(string resourcePath)
            where TConcrete : TContract
        {
            return ToTransientPrefabResource(typeof(TConcrete), resourcePath);
        }

        // Note: Here we assume that the contract is a component on the given prefab
        public BindingConditionSetter ToTransientPrefab<TConcrete>(GameObject prefab)
            where TConcrete : TContract
        {
            return ToTransientPrefab(typeof(TConcrete), prefab);
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToTransientGameObject<TConcrete>()
            where TConcrete : Component, TContract
        {
            return ToTransientGameObject(typeof(TConcrete));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject<TConcrete>()
            where TConcrete : Component, TContract
        {
            return ToSingleGameObject(typeof(TConcrete), null);
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject<TConcrete>(string concreteIdentifier)
            where TConcrete : Component, TContract
        {
            return ToSingleGameObject(typeof(TConcrete), concreteIdentifier);
        }

        public BindingConditionSetter ToSinglePrefab<TConcrete>(GameObject prefab)
            where TConcrete : TContract
        {
            return ToSinglePrefab(typeof(TConcrete), null, prefab);
        }

        public BindingConditionSetter ToSinglePrefab<TConcrete>(string concreteIdentifier, GameObject prefab)
            where TConcrete : TContract
        {
            return ToSinglePrefab(typeof(TConcrete), concreteIdentifier, prefab);
        }

        public BindingConditionSetter ToSinglePrefabResource<TConcrete>(string resourcePath)
            where TConcrete : TContract
        {
            return ToSinglePrefabResource(typeof(TConcrete), null, resourcePath);
        }

        public BindingConditionSetter ToSinglePrefabResource<TConcrete>(string concreteIdentifier, string resourcePath)
            where TConcrete : TContract
        {
            return ToSinglePrefabResource(typeof(TConcrete), concreteIdentifier, resourcePath);
        }
#endif
    }
}

