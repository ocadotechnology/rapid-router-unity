using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class TypeSingletonLazyCreator
    {
        readonly DiContainer _container;
        readonly SingletonId _id;
        readonly TypeSingletonProviderCreator _owner;

        int _referenceCount;
        object _instance;

        public TypeSingletonLazyCreator(
            SingletonId id, TypeSingletonProviderCreator owner,
            DiContainer container)
        {
            _container = container;
            _id = id;
            _owner = owner;
        }

        public object GetInstance(InjectContext context)
        {
            if (_instance == null)
            {
                InitInstance(context);
                Assert.IsNotNull(_instance);
            }

            return _instance;
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

        public IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return _container.ValidateObjectGraph(
                _id.ConcreteType, context, _id.ConcreteIdentifier);
        }

        void InitInstance(InjectContext context)
        {
            var concreteType = GetTypeToInstantiate(context.MemberType);

            bool autoInject = false;

            _instance = _container.InstantiateExplicit(
                concreteType, new List<TypeValuePair>(), context, _id.ConcreteIdentifier, autoInject);

            Assert.IsNotNull(_instance);

            // Inject after we've instantiated and set _instance so that we can support circular dependencies
            // as PostInject or field parameters
            _container.InjectExplicit(
                _instance, Enumerable.Empty<TypeValuePair>(), true,
                TypeAnalyzer.GetInfo(_instance.GetType()), context, _id.ConcreteIdentifier);
        }

        Type GetTypeToInstantiate(Type contractType)
        {
            if (_id.ConcreteType.IsOpenGenericType())
            {
                Assert.That(!contractType.IsAbstract());
                Assert.That(contractType.GetGenericTypeDefinition() == _id.ConcreteType);
                return contractType;
            }

            Assert.That(_id.ConcreteType.DerivesFromOrEqual(contractType));
            return _id.ConcreteType;
        }
    }
}
