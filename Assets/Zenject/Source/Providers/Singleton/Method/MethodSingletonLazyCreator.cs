using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public class MethodSingletonLazyCreator<TConcrete> : IMethodSingletonLazyCreator
    {
        readonly Func<InjectContext, TConcrete> _createMethod;
        readonly SingletonId _id;
        readonly MethodSingletonProviderCreator _owner;

        int _referenceCount;
        object _instance;

        public MethodSingletonLazyCreator(
            SingletonId id, MethodSingletonProviderCreator owner,
            Func<InjectContext, TConcrete> createMethod)
        {
            _owner = owner;
            _createMethod = createMethod;
            _id = id;
        }

        public Delegate CreateMethod
        {
            get
            {
                return _createMethod;
            }
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

        public object GetInstance(InjectContext context)
        {
            if (_instance == null)
            {
                _instance = _createMethod(context);

                if (_instance == null)
                {
                    throw new ZenjectResolveException(
                        "Unable to instantiate type '{0}' in SingletonLazyCreator".Fmt(context.MemberType));
                }
            }

            return _instance;
        }
    }
}

