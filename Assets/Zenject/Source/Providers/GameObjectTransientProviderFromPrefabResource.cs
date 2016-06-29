#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class GameObjectTransientProviderFromPrefabResource : ProviderBase
    {
        readonly string _resourcePath;
        readonly Type _concreteType;

        public GameObjectTransientProviderFromPrefabResource(
            Type concreteType, string resourcePath)
        {
            // Don't do this because it might be an interface
            //Assert.That(_concreteType.DerivesFrom<Component>());

            _concreteType = concreteType;
            _resourcePath = resourcePath;
        }

        public override Type GetInstanceType()
        {
            return _concreteType;
        }

        public override object GetInstance(InjectContext context)
        {
            Assert.That(_concreteType.DerivesFromOrEqual(context.MemberType));

            return context.Container.InstantiatePrefabResourceForComponent(_concreteType, _resourcePath);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return context.Container.ValidateObjectGraph(_concreteType, context);
        }
    }
}

#endif

