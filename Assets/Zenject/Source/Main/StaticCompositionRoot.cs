#if !ZEN_NOT_UNITY3D

namespace Zenject
{
    public static class StaticCompositionRoot
    {
        static DiContainer _container;

        static StaticCompositionRoot()
        {
            _container = new DiContainer();
        }

        public static DiContainer Container
        {
            get
            {
                return _container;
            }
        }
    }
}

#endif

