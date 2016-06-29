#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class MonoBehaviourSingletonId : IEquatable<MonoBehaviourSingletonId>
    {
        public readonly Type ConcreteType;
        public readonly string ConcreteIdentifier;
        public readonly GameObject GameObject;

        public MonoBehaviourSingletonId(Type concreteType, string concreteIdentifier, GameObject gameObject)
        {
            Assert.That(concreteType.DerivesFrom<Component>());
            Assert.IsNotNull(gameObject);

            ConcreteType = concreteType;
            GameObject = gameObject;
            ConcreteIdentifier = concreteIdentifier;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + this.ConcreteType.GetHashCode();
                hash = hash * 29 + (this.ConcreteIdentifier == null ? 0 : this.ConcreteIdentifier.GetHashCode());
                hash = hash * 29 + this.GameObject.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (other is MonoBehaviourSingletonId)
            {
                MonoBehaviourSingletonId otherId = (MonoBehaviourSingletonId)other;
                return otherId == this;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(MonoBehaviourSingletonId that)
        {
            return this == that;
        }

        public static bool operator ==(MonoBehaviourSingletonId left, MonoBehaviourSingletonId right)
        {
            return object.Equals(left.GameObject, right.GameObject) && object.Equals(left.ConcreteIdentifier, right.ConcreteIdentifier) && object.Equals(left.ConcreteType, right.ConcreteType);
        }

        public static bool operator !=(MonoBehaviourSingletonId left, MonoBehaviourSingletonId right)
        {
            return !left.Equals(right);
        }
    }
}

#endif
