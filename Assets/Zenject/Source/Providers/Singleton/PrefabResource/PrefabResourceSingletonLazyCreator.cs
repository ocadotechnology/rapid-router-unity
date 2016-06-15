#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;
using UnityEngine;

namespace Zenject
{
    public class PrefabResourceSingletonLazyCreator
    {
        readonly DiContainer _container;
        readonly PrefabResourceSingletonProviderCreator _owner;
        readonly PrefabResourceSingletonId _id;

        int _referenceCount;
        GameObject _rootObj;

        public PrefabResourceSingletonLazyCreator(
            DiContainer container, PrefabResourceSingletonProviderCreator owner,
            PrefabResourceSingletonId id)
        {
            _container = container;
            _owner = owner;
            _id = id;

            Assert.IsNotNull(id.ResourcePath);
        }

        public void IncRefCount()
        {
            _referenceCount += 1;
        }

        public void DecRefCount()
        {
            _referenceCount -= 1;

            if (_referenceCount <= 0)
            {
                _owner.RemoveCreator(_id);
            }
        }

        bool ContainsComponent(Type type)
        {
            var prefab = (GameObject)Resources.Load(_id.ResourcePath);
            return !prefab.GetComponentsInChildren(type, true).IsEmpty();
        }

        public object GetComponent(Type componentType, InjectContext context)
        {
            if (_rootObj == null)
            {
                Assert.IsNotNull(_id.ResourcePath);

                var prefab = (GameObject)Resources.Load(_id.ResourcePath);
                Assert.IsNotNull(prefab, "Could not find prefab at resource path '{0}'", _id.ResourcePath);

                _rootObj = (GameObject)GameObject.Instantiate(prefab);

                // Note that we always want to cache _container instead of using context.Container
                // since for singletons, the container they are accessed from should not determine
                // the container they are instantiated with
                // Transients can do that but not singletons

                _rootObj.transform.SetParent(_container.DefaultParent, false);

                _rootObj.SetActive(true);

                _container.InjectGameObject(_rootObj, true, false, new object[0], context);
            }

            var component = _rootObj.GetComponentInChildren(componentType);

            if (component == null)
            {
                throw new ZenjectResolveException(
                    "Could not find component with type '{0}' in given singleton prefab".Fmt(componentType));
            }

            return component;
        }

        public IEnumerable<ZenjectResolveException> ValidateBinding(
            Type componentType, InjectContext context)
        {
            if (!ContainsComponent(componentType))
            {
                yield return new ZenjectResolveException(
                    "Could not find component of type '{0}' in prefab with resource path '{1}' \nObject graph:\n{2}"
                    .Fmt(componentType.Name(), _id.ResourcePath, context.GetObjectGraphString()));
                yield break;
            }

            // In most cases componentType will be a MonoBehaviour but we also want to allow interfaces
            // And in that case we can't validate it
            if (!componentType.IsAbstract())
            {
                // Note that we always want to cache _container instead of using context.Container
                // since for singletons, the container they are accessed from should not determine
                // the container they are instantiated with
                // Transients can do that but not singletons
                foreach (var err in _container.ValidateObjectGraph(componentType, context))
                {
                    yield return err;
                }
            }
        }
    }
}

#endif
