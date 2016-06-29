using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    //[System.Diagnostics.DebuggerStepThrough]
    public class InstanceSingletonProviderCreator
    {
        readonly SingletonRegistry _singletonRegistry;
        readonly Dictionary<SingletonId, InstanceSingletonLazyCreator> _creators = new Dictionary<SingletonId, InstanceSingletonLazyCreator>();
        readonly DiContainer _container;

        public InstanceSingletonProviderCreator(
            DiContainer container,
            SingletonRegistry singletonRegistry)
        {
            _singletonRegistry = singletonRegistry;
            _container = container;
        }

        public void RemoveCreator(SingletonId id)
        {
            bool success = _creators.Remove(id);
            Assert.That(success);
        }

        InstanceSingletonLazyCreator AddCreator(SingletonId id, object instance)
        {
            InstanceSingletonLazyCreator creator;

            if (_creators.TryGetValue(id, out creator))
            {
                if (!ReferenceEquals(instance, creator.Instance))
                {
                    throw new ZenjectBindException(
                        "Cannot use 'ToSingleInstance' with multiple different instances!");
                }
            }
            else
            {
                creator = new InstanceSingletonLazyCreator(
                    id, this, _container, instance);
                _creators.Add(id, creator);
            }

            return creator;
        }

        public InstanceSingletonProvider CreateProvider(SingletonId singletonId, object instance)
        {
            var creator = AddCreator(singletonId, instance);
            return new InstanceSingletonProvider(creator, _singletonRegistry, singletonId);
        }
    }
}
