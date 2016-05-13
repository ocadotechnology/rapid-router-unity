using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class InstanceSingletonLazyCreator
    {
        readonly object _instance;
        readonly SingletonId _id;
        readonly InstanceSingletonProviderCreator _owner;

        int _referenceCount;

        public InstanceSingletonLazyCreator(
            SingletonId id, InstanceSingletonProviderCreator owner,
            DiContainer container, object instance)
        {
            Assert.That(instance != null || container.IsValidating);

            _owner = owner;
            _id = id;
            _instance = instance;
        }

        public object Instance
        {
            get
            {
                return _instance;
            }
        }

        public void DecRefCount()
        {
            _referenceCount -= 1;

            if (_referenceCount <= 0)
            {
                _owner.RemoveCreator(_id);
            }
        }

        public void IncRefCount()
        {
            _referenceCount += 1;
        }
    }
}


