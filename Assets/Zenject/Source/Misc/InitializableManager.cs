using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    // Responsibilities:
    // - Run Initialize() on all Iinitializable's, in the order specified by InitPriority
    public class InitializableManager
    {
        List<InitializableInfo> _initializables = new List<InitializableInfo>();

        public InitializableManager(
            [InjectOptional(InjectSources.Local)]
            List<IInitializable> initializables,
            [InjectOptional(InjectSources.Local)]
            List<ModestTree.Util.Tuple<Type, int>> priorities,
            DiContainer container)
        {
            foreach (var initializable in initializables)
            {
                // Note that we use zero for unspecified priority
                // This is nice because you can use negative or positive for before/after unspecified
                var matches = priorities.Where(x => initializable.GetType().DerivesFromOrEqual(x.First)).Select(x => x.Second).ToList();
                int priority = matches.IsEmpty() ? 0 : matches.Single();

                _initializables.Add(new InitializableInfo(initializable, priority));
            }
        }

        public void Initialize()
        {
            _initializables = _initializables.OrderBy(x => x.Priority).ToList();

            foreach (var initializable in _initializables.Select(x => x.Initializable).GetDuplicates())
            {
                Assert.That(false, "Found duplicate IInitializable with type '{0}'".Fmt(initializable.GetType()));
            }

            foreach (var initializable in _initializables)
            {
                Log.Debug("Initializing '" + initializable.Initializable.GetType() + "'");

                try
                {
#if PROFILING_ENABLED
                    using (ProfileBlock.Start("{0}.Initialize()", initializable.Initializable.GetType().Name()))
#endif
                    {
                        initializable.Initializable.Initialize();
                    }
                }
                catch (Exception e)
                {
                    throw new ZenjectException(
                        "Error occurred while initializing IInitializable with type '{0}'".Fmt(initializable.Initializable.GetType().Name()), e);
                }
            }
        }

        class InitializableInfo
        {
            public IInitializable Initializable;
            public int Priority;

            public InitializableInfo(IInitializable initializable, int priority)
            {
                Initializable = initializable;
                Priority = priority;
            }
        }
    }
}
