using System;
using System.Collections.Generic;

namespace Zenject
{
    [System.Diagnostics.DebuggerStepThrough]
    public abstract class ProviderBase : IDisposable
    {
        BindingCondition _condition = null;

        public BindingCondition Condition
        {
            get
            {
                return _condition;
            }
            set
            {
                _condition = value;
            }
        }

        public virtual bool Matches(InjectContext context)
        {
            return _condition == null || _condition(context);
        }

        // Return null if not applicable (for eg. if instance type is dependent on contractType)
        public abstract Type GetInstanceType();

        public abstract object GetInstance(InjectContext context);

        public abstract IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context);

        public virtual void Dispose()
        {
        }
    }
}
