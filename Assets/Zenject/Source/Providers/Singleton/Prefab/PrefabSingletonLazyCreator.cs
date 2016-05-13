#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;
using UnityEngine;

namespace Zenject
{
    public class PrefabSingletonLazyCreator
    {
        readonly DiContainer _container;
        readonly PrefabSingletonProviderCreator _owner;
        readonly PrefabSingletonId _id;

        int _referenceCount;
        GameObject _rootObj;

        public PrefabSingletonLazyCreator(
            DiContainer container, PrefabSingletonProviderCreator owner,
            PrefabSingletonId id)
        {
            _container = container;
            _owner = owner;
            _id = id;

            Assert.IsNotNull(id.Prefab);
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

        public bool ContainsComponent(Type type)
        {
            return !_id.Prefab.GetComponentsInChildren(type, true).IsEmpty();
        }

        public object GetComponent(Type componentType, InjectContext context)
        {
            if (_rootObj == null)
            {
                Assert.IsNotNull(_id.Prefab);

                _rootObj = (GameObject)GameObject.Instantiate(_id.Prefab);

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
                    "Could not find component of type '{0}' in prefab with name '{1}' \nObject graph:\n{2}"
                    .Fmt(componentType.Name(), _id.Prefab.name, context.GetObjectGraphString()));
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
