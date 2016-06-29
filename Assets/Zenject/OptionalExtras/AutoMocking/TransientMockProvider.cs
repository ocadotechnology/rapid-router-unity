#if !UNITY_WEBPLAYER && (NOT_UNITY || UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Zenject
{
    public class TransientMockProvider : ProviderBase
    {
        readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        public override Type GetInstanceType()
        {
            return null;
        }

        public override object GetInstance(InjectContext context)
        {
            object instance;

            if (!_instances.TryGetValue(context.MemberType, out instance))
            {
                instance = typeof(Mock).GetMethod("Of", new Type[] { context.MemberType }).Invoke(null, null);
                _instances.Add(context.MemberType, instance);
            }

            return instance;
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            // Always succeeds
            return Enumerable.Empty<ZenjectResolveException>();
        }
    }
}


#endif
