using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    //[System.Diagnostics.DebuggerStepThrough]
    public class MethodSingletonProviderCreator
    {
        readonly Dictionary<SingletonId, IMethodSingletonLazyCreator> _creators = new Dictionary<SingletonId, IMethodSingletonLazyCreator>();
        readonly SingletonRegistry _singletonRegistry;

        public MethodSingletonProviderCreator(
            SingletonRegistry singletonRegistry)
        {
            _singletonRegistry = singletonRegistry;
        }

        public void RemoveCreator(SingletonId id)
        {
            bool success = _creators.Remove(id);
            Assert.That(success);
        }

        IMethodSingletonLazyCreator AddCreator<TConcrete>(
            SingletonId id, Func<InjectContext, TConcrete> method)
        {
            Assert.IsEqual(id.ConcreteType, typeof(TConcrete));

            IMethodSingletonLazyCreator creator;

            if (_creators.TryGetValue(id, out creator))
            {
                if (!AreFunctionsEqual(creator.CreateMethod, method))
                {
                    throw new ZenjectBindException(
                        "Cannot use 'ToSingleMethod' with multiple different methods!");
                }
            }
            else
            {
                creator = new MethodSingletonLazyCreator<TConcrete>(id, this, method);
                _creators.Add(id, creator);
            }

            return creator;
        }

        bool AreFunctionsEqual(Delegate left, Delegate right)
        {
            return left.Target == right.Target && left.Method() == right.Method();
        }

        public MethodSingletonProvider CreateProvider<TConcrete>(
            string concreteIdentifier, Func<InjectContext, TConcrete> method)
        {
            var singletonId = new SingletonId(typeof(TConcrete), concreteIdentifier);
            var creator = AddCreator<TConcrete>(singletonId, method);

            return new MethodSingletonProvider(
                creator, singletonId, _singletonRegistry);
        }
    }
}
