#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class PrefabResourceSingletonProviderCreator
    {
        readonly SingletonRegistry _singletonRegistry;
        readonly Dictionary<PrefabResourceSingletonId, PrefabResourceSingletonLazyCreator> _creators = new Dictionary<PrefabResourceSingletonId, PrefabResourceSingletonLazyCreator>();
        readonly DiContainer _container;
        readonly Dictionary<SingletonId, ResourceMarkInfo> _resourceMarks = new Dictionary<SingletonId, ResourceMarkInfo>();

        public PrefabResourceSingletonProviderCreator(
            DiContainer container, SingletonRegistry singletonRegistry)
        {
            _singletonRegistry = singletonRegistry;
            _container = container;
        }

        // Need to do this to ensure that we don't use multiple different prefabs
        // with the same singleton ID
        public void MarkResource(SingletonId id, string resourcePath)
        {
            ResourceMarkInfo markInfo;

            if (_resourceMarks.TryGetValue(id, out markInfo))
            {
                if (markInfo.ResourcePath != resourcePath)
                {
                    throw new ZenjectBindException(
                        "Attempted to use multiple different resource paths with ToSinglePrefabResource using the same type/identifier: '{0}' / '{1}'"
                        .Fmt(id.ConcreteType, id.ConcreteIdentifier));
                }
            }
            else
            {
                markInfo = new ResourceMarkInfo()
                {
                    ResourcePath = resourcePath,
                };
                _resourceMarks.Add(id, markInfo);
            }

            markInfo.RefCount += 1;
        }

        public void UnmarkResource(SingletonId id, string resourcePath)
        {
            var markInfo = _resourceMarks[id];

            Assert.IsEqual(markInfo.ResourcePath, resourcePath);

            markInfo.RefCount -= 1;

            if (markInfo.RefCount == 0)
            {
                _resourceMarks.RemoveWithConfirm(id);
            }
        }

        public void RemoveCreator(PrefabResourceSingletonId id)
        {
            bool success = _creators.Remove(id);
            Assert.That(success);
        }

        PrefabResourceSingletonLazyCreator AddCreator(PrefabResourceSingletonId id)
        {
            PrefabResourceSingletonLazyCreator creator;

            if (!_creators.TryGetValue(id, out creator))
            {
                creator = new PrefabResourceSingletonLazyCreator(_container, this, id);
                _creators.Add(id, creator);
            }

            return creator;
        }

        public PrefabResourceSingletonProvider CreateProvider(
            string concreteIdentifier, Type concreteType, string resourcePath)
        {
            var id = new PrefabResourceSingletonId(concreteIdentifier, resourcePath);
            var creator = AddCreator(id);

            return new PrefabResourceSingletonProvider(
                id, concreteType, creator, _singletonRegistry, this);
        }

        class ResourceMarkInfo
        {
            public string ResourcePath;
            public int RefCount;
        }
    }
}
#endif
