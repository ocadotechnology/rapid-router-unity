#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class PrefabResourceSingletonId : IEquatable<PrefabResourceSingletonId>
    {
        public readonly string ConcreteIdentifier;
        public readonly string ResourcePath;

        public PrefabResourceSingletonId(string concreteIdentifier, string resourcePath)
        {
            Assert.IsNotNull(resourcePath);

            ConcreteIdentifier = concreteIdentifier;
            ResourcePath = resourcePath;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 29 + (this.ConcreteIdentifier == null ? 0 : this.ConcreteIdentifier.GetHashCode());
                hash = hash * 29 + (this.ResourcePath == null ? 0 : this.ResourcePath.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object other)
        {
            if (other is PrefabResourceSingletonId)
            {
                PrefabResourceSingletonId otherId = (PrefabResourceSingletonId)other;
                return otherId == this;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(PrefabResourceSingletonId that)
        {
            return this == that;
        }

        public static bool operator ==(PrefabResourceSingletonId left, PrefabResourceSingletonId right)
        {
            return object.Equals(left.ConcreteIdentifier, right.ConcreteIdentifier) && object.Equals(left.ResourcePath, right.ResourcePath);
        }

        public static bool operator !=(PrefabResourceSingletonId left, PrefabResourceSingletonId right)
        {
            return !left.Equals(right);
        }
    }
}

#endif
