#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class GameObjectTransientProviderFromPrefab : ProviderBase
    {
        readonly Type _concreteType;
        readonly GameObject _template;

        public GameObjectTransientProviderFromPrefab(
            Type concreteType, GameObject template, DiContainer container)
        {
            // Don't do this because it might be an interface
            //Assert.That(typeof(T).DerivesFrom<Component>());

            _concreteType = concreteType;
            _template = template;

            var singletonMark = container.SingletonRegistry.TryGetSingletonType(concreteType);

            if (singletonMark.HasValue)
            {
                throw new ZenjectBindException(
                    "Attempted to use 'ToTransientPrefab' with the same type ('{0}') that is already marked with '{1}'".Fmt(concreteType.Name(), singletonMark.Value));
            }
        }

        public override Type GetInstanceType()
        {
            return _concreteType;
        }

        public override object GetInstance(InjectContext context)
        {
            Assert.That(_concreteType.DerivesFromOrEqual(context.MemberType));
            return context.Container.InstantiatePrefabForComponent(_concreteType, _template);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return context.Container.ValidateObjectGraph(_concreteType, context);
        }
    }
}

#endif
