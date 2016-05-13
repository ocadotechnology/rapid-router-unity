using System;
using ModestTree;
#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public abstract class TypeBinder : BinderBase
    {
        public TypeBinder(
            DiContainer container,
            Type contractType,
            string bindIdentifier)
            : base(container, contractType, bindIdentifier)
        {
        }

        public BindingConditionSetter ToTransient()
        {
#if !ZEN_NOT_UNITY3D
            if (ContractType.DerivesFrom(typeof(Component)))
            {
                throw new ZenjectBindException(
                    "Should not use ToTransient for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToTransientFromPrefab"
                    .Fmt(ContractType.Name()));
            }
#endif

            return ToProvider(new TransientProvider(ContractType, Container));
        }

        public BindingConditionSetter ToTransient(Type concreteType)
        {
#if !ZEN_NOT_UNITY3D
            if (concreteType.DerivesFrom(typeof(Component)))
            {
                throw new ZenjectBindException(
                    "Should not use ToTransient for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToTransientFromPrefab"
                    .Fmt(concreteType.Name()));
            }
#endif

            return ToProvider(new TransientProvider(concreteType, Container));
        }

        public BindingConditionSetter ToSingle()
        {
            return ToSingle((string)null);
        }

        public BindingConditionSetter ToSingle(string concreteIdentifier)
        {
#if !ZEN_NOT_UNITY3D
            if (ContractType.DerivesFrom(typeof(Component)))
            {
                throw new ZenjectBindException(
                    "Should not use ToSingle for Monobehaviours (when binding type '{0}'), you probably want either ToLookup or ToSinglePrefab or ToSingleGameObject"
                    .Fmt(ContractType.Name()));
            }
#endif

            return ToProvider(Container.SingletonProviderCreator.CreateProviderFromType(concreteIdentifier, ContractType));
        }

        public BindingConditionSetter ToSingle(Type concreteType)
        {
            return ToSingle(null, concreteType);
        }

        public BindingConditionSetter ToSingle(string concreteIdentifier, Type concreteType)
        {
            if (!concreteType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), ContractType.Name()));
            }

            return ToProvider(Container.SingletonProviderCreator.CreateProviderFromType(concreteIdentifier, concreteType));
        }

        public BindingConditionSetter ToSingle(Type concreteType, string concreteIdentifier)
        {
            if (!concreteType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), ContractType.Name()));
            }

#if !ZEN_NOT_UNITY3D
            if (concreteType.DerivesFrom(typeof(Component)))
            {
                throw new ZenjectBindException(
                    "Should not use ToSingle for Monobehaviours (when binding type '{0}' to '{1}'), you probably want either ToLookup or ToSinglePrefab or ToSinglePrefabResource or ToSingleGameObject"
                    .Fmt(ContractType.Name(), concreteType.Name()));
            }
#endif

            return ToProvider(Container.SingletonProviderCreator.CreateProviderFromType(concreteIdentifier, concreteType));
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToSingleMonoBehaviour(GameObject gameObject)
        {
            return ToSingleMonoBehaviour(null, ContractType, gameObject);
        }

        public BindingConditionSetter ToSingleMonoBehaviour(Type concreteType, GameObject gameObject)
        {
            return ToSingleMonoBehaviour(null, concreteType, gameObject);
        }

        public BindingConditionSetter ToSingleMonoBehaviour(
            string concreteIdentifier, Type concreteType, GameObject gameObject)
        {
            if (!concreteType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), ContractType.Name()));
            }

            if (!concreteType.DerivesFrom(typeof(Component)))
            {
                throw new ZenjectBindException(
                    "Expected type '{0}' to derive from UnityEngine.Component".Fmt(concreteType.Name()));
            }

            return ToProvider(Container.SingletonProviderCreator.CreateProviderFromMonoBehaviour(concreteIdentifier, concreteType, gameObject));
        }

        // Note that concreteType here could be an interface as well
        public BindingConditionSetter ToSinglePrefab(
            Type concreteType, string concreteIdentifier, GameObject prefab)
        {
            if (!concreteType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), ContractType.Name()));
            }

            if (ZenUtil.IsNull(prefab))
            {
                throw new ZenjectBindException(
                    "Received null prefab while binding type '{0}'".Fmt(concreteType.Name()));
            }

            return ToProvider(
                Container.SingletonProviderCreator.CreateProviderFromPrefab(concreteIdentifier, concreteType, prefab));
        }

        public BindingConditionSetter ToTransientPrefab(Type concreteType, GameObject prefab)
        {
            if (!concreteType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), ContractType.Name()));
            }

            // We have to cast to object otherwise we get SecurityExceptions when this function is run outside of unity
            if (ZenUtil.IsNull(prefab))
            {
                throw new ZenjectBindException("Received null prefab while binding type '{0}'".Fmt(concreteType.Name()));
            }

            return ToProvider(new GameObjectTransientProviderFromPrefab(concreteType, prefab, Container));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToTransientGameObject()
        {
            if (!ContractType.DerivesFrom(typeof(Component)))
            {
                throw new ZenjectBindException(
                    "Expected UnityEngine.Component derived type when binding type '{0}'".Fmt(ContractType.Name()));
            }

            return ToProvider(new GameObjectTransientProvider(ContractType));
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToTransientGameObject(Type concreteType)
        {
            if (!concreteType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), ContractType.Name()));
            }

            return ToProvider(new GameObjectTransientProvider(concreteType));
        }

        public BindingConditionSetter ToSingleGameObject()
        {
            return ToSingleGameObject(null);
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject(string concreteIdentifier)
        {
            return ToSingleGameObject(ContractType, concreteIdentifier);
        }

        // Creates a new game object and adds the given type as a new component on it
        public BindingConditionSetter ToSingleGameObject(Type concreteType, string concreteIdentifier)
        {
            if (!concreteType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), ContractType.Name()));
            }

            if (!concreteType.DerivesFrom<Component>())
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from UnityEngine.Component".Fmt(concreteType.Name()));
            }

            return ToProvider(
                Container.SingletonProviderCreator.CreateProviderFromGameObject(concreteType, concreteIdentifier));
        }

        public BindingConditionSetter ToTransientPrefabResource(string resourcePath)
        {
            return ToTransientPrefabResource(ContractType, resourcePath);
        }

        public BindingConditionSetter ToTransientPrefabResource(Type concreteType, string resourcePath)
        {
            Assert.IsNotNull(resourcePath);
            return ToProvider(new GameObjectTransientProviderFromPrefabResource(concreteType, resourcePath));
        }

        public BindingConditionSetter ToSinglePrefabResource(Type concreteType, string concreteIdentifier, string resourcePath)
        {
            Assert.That(concreteType.DerivesFromOrEqual(ContractType));
            Assert.IsNotNull(resourcePath);

            return ToProvider(
                Container.SingletonProviderCreator.CreateProviderFromPrefabResource(concreteIdentifier, concreteType, resourcePath));
        }

        public BindingConditionSetter ToSinglePrefabResource(string resourcePath)
        {
            return ToSinglePrefabResource(null, resourcePath);
        }

        public BindingConditionSetter ToSinglePrefabResource(string concreteIdentifier, string resourcePath)
        {
            return ToSinglePrefabResource(ContractType, concreteIdentifier, resourcePath);
        }

        public BindingConditionSetter ToTransientPrefab(GameObject prefab)
        {
            return ToTransientPrefab(ContractType, prefab);
        }

        public BindingConditionSetter ToSinglePrefab(GameObject prefab)
        {
            return ToSinglePrefab(null, prefab);
        }

        public BindingConditionSetter ToSinglePrefab(string concreteIdentifier, GameObject prefab)
        {
            return ToSinglePrefab(ContractType, concreteIdentifier, prefab);
        }

#endif
        protected BindingConditionSetter ToSingleMethodBase<TConcrete>(string concreteIdentifier, Func<InjectContext, TConcrete> method)
        {
            return ToProvider(Container.SingletonProviderCreator.CreateProviderFromMethod(concreteIdentifier, method));
        }

        protected BindingConditionSetter ToSingleFactoryBase<TConcrete, TFactory>(string concreteIdentifier)
            where TFactory : IFactory<TConcrete>
        {
            return ToProvider(Container.SingletonProviderCreator.CreateProviderFromFactory<TConcrete, TFactory>(concreteIdentifier));
        }

        protected BindingConditionSetter ToMethodBase<T>(Func<InjectContext, T> method)
        {
            if (!typeof(T).DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(typeof(T), ContractType.Name()));
            }

            return ToProvider(new MethodProvider<T>(method, Container));
        }

        protected BindingConditionSetter ToLookupBase<TConcrete>(string identifier)
        {
            return ToMethodBase<TConcrete>((ctx) => ctx.Container.Resolve<TConcrete>(
                new InjectContext(
                    ctx.Container, typeof(TConcrete), identifier,
                    false, ctx.ObjectType, ctx.ObjectInstance, ctx.MemberName, ctx, null, ctx.FallBackValue, ctx.SourceType)));
        }

        protected BindingConditionSetter ToGetterBase<TObj, TResult>(string identifier, Func<TObj, TResult> method)
        {
            return ToMethodBase((ctx) => method(ctx.Container.Resolve<TObj>(
                new InjectContext(
                    ctx.Container, typeof(TObj), identifier,
                    false, ctx.ObjectType, ctx.ObjectInstance, ctx.MemberName, ctx, null, ctx.FallBackValue, ctx.SourceType))));
        }

        public BindingConditionSetter ToInstance(Type concreteType, object instance)
        {
            if (ZenUtil.IsNull(instance) && !Container.IsValidating)
            {
                string message;

                if (ContractType == concreteType)
                {
                    message = "Received null instance during Bind command with type '{0}'".Fmt(ContractType.Name());
                }
                else
                {
                    message =
                        "Received null instance during Bind command when binding type '{0}' to '{1}'".Fmt(ContractType.Name(), concreteType.Name());
                }

                throw new ZenjectBindException(message);
            }

            if (!ZenUtil.IsNull(instance) && !instance.GetType().DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), ContractType.Name()));
            }

            return ToProvider(new InstanceProvider(concreteType, instance, Container));
        }

        protected BindingConditionSetter ToSingleInstance(Type concreteType, string concreteIdentifier, object instance)
        {
            if (!concreteType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), ContractType.Name()));
            }

            if (ZenUtil.IsNull(instance) && !Container.IsValidating)
            {
                string message;

                if (ContractType == concreteType)
                {
                    message = "Received null singleton instance during Bind command with type '{0}'".Fmt(ContractType.Name());
                }
                else
                {
                    message =
                        "Received null singleton instance during Bind command when binding type '{0}' to '{1}'".Fmt(ContractType.Name(), concreteType.Name());
                }

                throw new ZenjectBindException(message);
            }

            return ToProvider(Container.SingletonProviderCreator.CreateProviderFromInstance(concreteIdentifier, concreteType, instance));
        }

#if !ZEN_NOT_UNITY3D

        public BindingConditionSetter ToResource(string resourcePath)
        {
            return ToResource(ContractType, resourcePath);
        }

        public BindingConditionSetter ToResource(Type concreteType, string resourcePath)
        {
            if (!concreteType.DerivesFromOrEqual(ContractType))
            {
                throw new ZenjectBindException(
                    "Invalid type given during bind command.  Expected type '{0}' to derive from type '{1}'".Fmt(concreteType.Name(), ContractType.Name()));
            }

            return ToProvider(new ResourceProvider(concreteType, resourcePath));
        }
#endif
    }
}

