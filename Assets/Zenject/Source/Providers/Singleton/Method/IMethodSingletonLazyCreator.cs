using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    public interface IMethodSingletonLazyCreator
    {
        void DecRefCount();
        void IncRefCount();

        object GetInstance(InjectContext context);

        Delegate CreateMethod
        {
            get;
        }
    }
}


