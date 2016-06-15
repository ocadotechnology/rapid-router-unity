using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

#if !ZEN_NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public enum SingletonTypes
    {
        ToSingle,
        ToSingleMethod,
        ToSingleInstance,
        ToSinglePrefab,
        ToSinglePrefabResource,
        ToSingleFactory,
        ToSingleGameObject,
        ToSingleMonoBehaviour,
    }

    public class SingletonRegistry
    {
        readonly Dictionary<SingletonId, SingletonInfo> _singletonInfos = new Dictionary<SingletonId, SingletonInfo>();

        public SingletonTypes? TryGetSingletonType<T>()
        {
            return TryGetSingletonType(typeof(T));
        }

        public SingletonTypes? TryGetSingletonType(Type type)
        {
            return TryGetSingletonType(type, null);
        }

        public SingletonTypes? TryGetSingletonType(Type type, string concreteIdentifier)
        {
            return TryGetSingletonType(new SingletonId(type, concreteIdentifier));
        }

        public SingletonTypes? TryGetSingletonType(SingletonId id)
        {
            SingletonInfo info;

            if (_singletonInfos.TryGetValue(id, out info))
            {
                return info.Type;
            }

            return null;
        }

        public void MarkSingleton(
            Type type, string concreteIdentifier, SingletonTypes singletonTyp)
        {
            MarkSingleton(new SingletonId(type, concreteIdentifier), singletonTyp);
        }

        public void MarkSingleton(SingletonId id, SingletonTypes type)
        {
            SingletonInfo info;

            if (!_singletonInfos.TryGetValue(id, out info))
            {
                info = new SingletonInfo()
                {
                    Type = type,
                };

                _singletonInfos.Add(id, info);
            }

            if (info.Type != type)
            {
                throw new ZenjectBindException(
                    "Cannot use both '{0}' and '{1}' for the same type/concreteIdentifier!".Fmt(info.Type, type));
            }

            info.RefCount += 1;
        }

        public void UnmarkSingleton(
            Type type, string concreteIdentifier, SingletonTypes singletonTyp)
        {
            UnmarkSingleton(
                new SingletonId(type, concreteIdentifier), singletonTyp);
        }

        public void UnmarkSingleton(SingletonId id, SingletonTypes type)
        {
            Assert.That(_singletonInfos.ContainsKey(id));

            var info = _singletonInfos[id];

            Assert.IsEqual(type, info.Type);

            info.RefCount -= 1;

            if (info.RefCount == 0)
            {
                _singletonInfos.RemoveWithConfirm(id);
            }
        }

        class SingletonInfo
        {
            public SingletonTypes Type;
            public int RefCount;
        }
    }
}

