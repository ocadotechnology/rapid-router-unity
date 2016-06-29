#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    public class GameObjectTransientProvider : ProviderBase
    {
        readonly Type _componentType;

        public GameObjectTransientProvider(Type componentType)
        {
            Assert.That(componentType.DerivesFrom<Component>());
            _componentType = componentType;
        }

        public override Type GetInstanceType()
        {
            return _componentType;
        }

        public override object GetInstance(InjectContext context)
        {
            Assert.That(_componentType.DerivesFromOrEqual(context.MemberType));

            return context.Container.InstantiateComponentOnNewGameObjectExplicit(
                _componentType, _componentType.Name(), new List<TypeValuePair>(), context);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return context.Container.ValidateObjectGraph(_componentType, context);
        }
    }
}

#endif

