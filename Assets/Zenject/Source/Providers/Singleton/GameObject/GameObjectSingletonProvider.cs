#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    // NOTE: we need the provider seperate from the creator because
    // if we return the same provider multiple times then the condition
    // will get over-written
    public class GameObjectSingletonProvider : ProviderBase
    {
        readonly SingletonRegistry _singletonRegistry;
        readonly SingletonId _id;
        readonly GameObjectSingletonLazyCreator _creator;

        public GameObjectSingletonProvider(
            GameObjectSingletonLazyCreator creator,
            SingletonId id,
            SingletonRegistry singletonRegistry)
        {
            _singletonRegistry = singletonRegistry;
            _id = id;
            _creator = creator;

            Init();
        }

        void Init()
        {
            _singletonRegistry.MarkSingleton(_id, SingletonTypes.ToSingleGameObject);
            _creator.IncRefCount();
        }

        public override void Dispose()
        {
            _creator.DecRefCount();
            _singletonRegistry.UnmarkSingleton(_id, SingletonTypes.ToSingleGameObject);
        }

        public override Type GetInstanceType()
        {
            return _id.ConcreteType;
        }

        public override object GetInstance(InjectContext context)
        {
            return _creator.GetInstance(context);
        }

        public override IEnumerable<ZenjectResolveException> ValidateBinding(InjectContext context)
        {
            return _creator.ValidateBinding(context);
        }
    }
}

#endif

