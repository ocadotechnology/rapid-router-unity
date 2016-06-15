using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public interface IFactorySingletonLazyCreator
    {
        IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context);
        object GetInstance(InjectContext context);
        void DecRefCount();
        void IncRefCount();
    }
}

