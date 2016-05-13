#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class PrefabSingletonProviderCreator
    {
        readonly SingletonRegistry _singletonRegistry;
        readonly Dictionary<PrefabSingletonId, PrefabSingletonLazyCreator> _creators = new Dictionary<PrefabSingletonId, PrefabSingletonLazyCreator>();
        readonly DiContainer _container;
        readonly Dictionary<SingletonId, PrefabMarkInfo> _prefabMarks = new Dictionary<SingletonId, PrefabMarkInfo>();

        public PrefabSingletonProviderCreator(
            DiContainer container,
            SingletonRegistry singletonRegistry)
        {
            _singletonRegistry = singletonRegistry;
            _container = container;
        }

        // Need to do this to ensure that we don't use multiple different prefabs
        // with the same singleton ID
        public void MarkPrefab(SingletonId id, GameObject prefab)
        {
            PrefabMarkInfo markInfo;

            if (_prefabMarks.TryGetValue(id, out markInfo))
            {
                if (markInfo.Prefab != prefab)
                {
                    throw new ZenjectBindException(
                        "Attempted to use multiple different prefabs with ToSinglePrefab using the same type/identifier: '{0}' / '{1}'"
                        .Fmt(id.ConcreteType, id.ConcreteIdentifier));
                }
            }
            else
            {
                markInfo = new PrefabMarkInfo()
                {
                    Prefab = prefab,
                };
                _prefabMarks.Add(id, markInfo);
            }

            markInfo.RefCount += 1;
        }

        public void UnmarkPrefab(SingletonId id, GameObject prefab)
        {
            var markInfo = _prefabMarks[id];

            Assert.IsEqual(markInfo.Prefab, prefab);

            markInfo.RefCount -= 1;

            if (markInfo.RefCount == 0)
            {
                _prefabMarks.RemoveWithConfirm(id);
            }
        }

        internal void RemoveCreator(PrefabSingletonId id)
        {
            bool success = _creators.Remove(id);
            Assert.That(success);
        }

        PrefabSingletonLazyCreator AddCreator(PrefabSingletonId id)
        {
            PrefabSingletonLazyCreator creator;

            if (!_creators.TryGetValue(id, out creator))
            {
                creator = new PrefabSingletonLazyCreator(_container, this, id);
                _creators.Add(id, creator);
            }

            return creator;
        }

        public PrefabSingletonProvider CreateProvider(
            string concreteIdentifier, Type concreteType, GameObject prefab)
        {
            var id = new PrefabSingletonId(concreteIdentifier, prefab);
            var creator = AddCreator(id);

            return new PrefabSingletonProvider(
                id, concreteType, creator, _singletonRegistry, this);
        }

        class PrefabMarkInfo
        {
            public GameObject Prefab;
            public int RefCount;
        }
    }
}
#endif
