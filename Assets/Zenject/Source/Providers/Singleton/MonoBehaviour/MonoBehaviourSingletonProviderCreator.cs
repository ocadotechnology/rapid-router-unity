#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public class MonoBehaviourSingletonProviderCreator
    {
        readonly SingletonRegistry _singletonRegistry;
        readonly Dictionary<MonoBehaviourSingletonId, MonoBehaviourSingletonLazyCreator> _creators = new Dictionary<MonoBehaviourSingletonId, MonoBehaviourSingletonLazyCreator>();
        readonly DiContainer _container;
        readonly Dictionary<SingletonId, GameObjectMarkInfo> _gameObjectMarks = new Dictionary<SingletonId, GameObjectMarkInfo>();

        public MonoBehaviourSingletonProviderCreator(
            DiContainer container,
            SingletonRegistry singletonRegistry)
        {
            _singletonRegistry = singletonRegistry;
            _container = container;
        }

        // Need to do this to ensure that we don't use multiple different ToMonoBehaviour
        // bindings with different game objects but same type/id
        public void MarkGameObject(SingletonId id, GameObject gameObject)
        {
            GameObjectMarkInfo markInfo;

            if (_gameObjectMarks.TryGetValue(id, out markInfo))
            {
                if (markInfo.GameObject != gameObject)
                {
                    throw new ZenjectBindException(
                        "Attempted to use multiple different Game Objects with ToSingleMonoBehaviour using the same type/identifier: '{0}' / '{1}'"
                        .Fmt(id.ConcreteType, id.ConcreteIdentifier));
                }
            }
            else
            {
                markInfo = new GameObjectMarkInfo()
                {
                    GameObject = gameObject,
                };
                _gameObjectMarks.Add(id, markInfo);
            }

            markInfo.RefCount += 1;
        }

        public void UnmarkGameObject(SingletonId id, GameObject gameObject)
        {
            var markInfo = _gameObjectMarks[id];

            Assert.IsEqual(markInfo.GameObject, gameObject);

            markInfo.RefCount -= 1;

            if (markInfo.RefCount == 0)
            {
                _gameObjectMarks.RemoveWithConfirm(id);
            }
        }

        internal void RemoveCreator(MonoBehaviourSingletonId id)
        {
            bool success = _creators.Remove(id);
            Assert.That(success);
        }

        MonoBehaviourSingletonLazyCreator AddCreator(MonoBehaviourSingletonId id)
        {
            MonoBehaviourSingletonLazyCreator creator;

            if (!_creators.TryGetValue(id, out creator))
            {
                creator = new MonoBehaviourSingletonLazyCreator(_container, this, id);
                _creators.Add(id, creator);
            }

            return creator;
        }

        public ProviderBase CreateProvider(
            string concreteIdentifier, Type componentType, GameObject gameObject)
        {
            var id = new MonoBehaviourSingletonId(componentType, concreteIdentifier, gameObject);
            var lazyCreator = AddCreator(id);

            return new MonoBehaviourSingletonProvider(
                id, componentType, lazyCreator, _singletonRegistry, this);
        }

        class GameObjectMarkInfo
        {
            public GameObject GameObject;
            public int RefCount;
        }
    }
}
#endif

