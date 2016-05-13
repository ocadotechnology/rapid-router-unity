#if !ZEN_NOT_UNITY3D

#pragma warning disable 414
using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;

#if UNITY_5_3
using UnityEngine.SceneManagement;
#endif

namespace Zenject
{
    public class SceneCompositionRoot : CompositionRoot
    {
        public static readonly List<Scene> DecoratedScenes = new List<Scene>();
        public static Action<DiContainer> BeforeInstallHooks;
        public static Action<DiContainer> AfterInstallHooks;

        [Tooltip("When true, inactive objects will not have their members injected")]
        public bool OnlyInjectWhenActive = false;

        [Tooltip("When true, objects that are created at runtime will be parented to the SceneCompositionRoot")]
        public bool ParentNewObjectsUnderRoot = true;

        [SerializeField]
        public MonoInstaller[] Installers = new MonoInstaller[0];

        DiContainer _container;
        IFacade _rootFacade = null;

        static StaticSettings _staticSettings;

        public override DiContainer Container
        {
            get
            {
                return _container;
            }
        }

        public override IFacade RootFacade
        {
            get
            {
                return _rootFacade;
            }
        }

        public override bool AllowInjectInactive
        {
            get
            {
                return !OnlyInjectWhenActive;
            }
        }

        public void Awake()
        {
            var extraInstallers = new List<IInstaller>();

            if (_staticSettings != null)
            // Static settings are needed if creating a SceneCompositionRoot dynamically
            {
                extraInstallers = _staticSettings.Installers;
                OnlyInjectWhenActive = _staticSettings.OnlyInjectWhenActive;
                ParentNewObjectsUnderRoot = _staticSettings.ParentNewObjectsUnderRoot;
                _staticSettings = null;
            }

            // We always want to initialize GlobalCompositionRoot as early as possible
            GlobalCompositionRoot.Instance.EnsureIsInitialized();

            Assert.IsNull(Container);
            Assert.IsNull(RootFacade);

            Log.Debug("Initializing SceneCompositionRoot in scene '{0}'", this.gameObject.scene.name);
            _container = CreateContainer(
                false, GlobalCompositionRoot.Instance.Container, extraInstallers);

            Log.Debug("SceneCompositionRoot: Finished install phase.  Injecting into scene...");
            InjectObjectsInScene();

            Log.Debug("SceneCompositionRoot: Resolving root IFacade...");
            _rootFacade = _container.Resolve<IFacade>();

            DecoratedScenes.Clear();

            Assert.IsNotNull(Container);
            Assert.IsNotNull(RootFacade);
        }

        public void Start()
        {
            // Always run the IInitializable's at the very beginning of Start()
            // This file (SceneCompositionRoot) should always have the earliest execution order (see SceneCompositionRoot.cs.meta)
            // This is necessary in some edge cases where parts of Unity do not work the same during Awake() as they do in Start/Update
            // For example, changing rectTransform.localPosition does not automatically update rectTransform.position in some cases
            // Also, most people treat Awake() as very minimal initialization, such as setting up a valid state for the
            // object, initializing variables, etc. and treat Start() as the place where more complex initialization occurs,
            // so this is consistent with that convention as well
            GlobalCompositionRoot.Instance.InitializeRootIfNecessary();
            _rootFacade.Initialize();
        }

        public DiContainer CreateContainer(
            bool isValidating, DiContainer parentContainer, List<IInstaller> extraInstallers)
        {
            var container = new DiContainer(parentContainer);

            container.IsValidating = isValidating;

            container.Bind<CompositionRoot>().ToInstance(this);
            container.Bind<SceneCompositionRoot>().ToInstance(this);

            if (ParentNewObjectsUnderRoot)
            {
                container.Bind<Transform>(ZenConstants.DefaultParentId)
                    .ToInstance<Transform>(this.transform);
            }

            if (BeforeInstallHooks != null)
            {
                BeforeInstallHooks(container);
                // Reset extra bindings for next time we change scenes
                BeforeInstallHooks = null;
            }

            container.Install<StandardInstaller>();

            var allInstallers = extraInstallers.Concat(Installers).ToList();

            if (allInstallers.Where(x => x != null).IsEmpty())
            {
                Log.Warn("No installers found while initializing SceneCompositionRoot");
            }
            else
            {
                container.Install(allInstallers);
            }

            if (AfterInstallHooks != null)
            {
                AfterInstallHooks(container);
                // Reset extra bindings for next time we change scenes
                AfterInstallHooks = null;
            }

            return container;
        }

        public static IEnumerable<GameObject> GetSceneRootObjects(Scene scene, bool includeInactive)
        {
            // Note: We can't use activeScene.GetRootObjects() here because that apparently fails with an exception
            // about the scene not being loaded yet when executed in Awake
            // It's important here that we only inject into root objects that are part of our scene
            // Otherwise, if there is an object that is marked with DontDestroyOnLoad, then it will
            // be injected multiple times when another scene is loaded
            // Also make sure not to inject into the global root objects which are handled in GlobalCompositionRoot
            return Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(x => (includeInactive || x.activeSelf) && x.transform.parent == null && x.GetComponent<GlobalCompositionRoot>() == null && (x.scene == scene || DecoratedScenes.Contains(x.scene)));
        }

        void InjectObjectsInScene()
        {
            Log.Debug("Injecting all objects in scene '{0}'", this.gameObject.scene.name);

            foreach (var rootObj in GetSceneRootObjects(this.gameObject.scene, !OnlyInjectWhenActive))
            {
                _container.InjectGameObject(rootObj, true, !OnlyInjectWhenActive);
            }
        }

        // These methods can be used for cases where you need to create the SceneCompositionRoot entirely in code
        // Necessary because the Awake() method is called immediately after InstantiateComponent<SceneCompositionRoot>
        // so there's no other way to add installers to it
        public static SceneCompositionRoot Instantiate(
            GameObject parent, StaticSettings settings)
        {
            var gameObject = new GameObject();

            if (parent != null)
            {
                gameObject.transform.SetParent(parent.transform, false);
            }

            return InstantiateComponent(gameObject, settings);
        }

        public static SceneCompositionRoot InstantiateComponent(
            GameObject gameObject, StaticSettings settings)
        {
            Assert.IsNull(_staticSettings);
            _staticSettings = settings;

            var result = gameObject.AddComponent<SceneCompositionRoot>();
            Assert.IsNull(_staticSettings);
            return result;
        }

        public class StaticSettings
        {
            public List<IInstaller> Installers;
            public bool ParentNewObjectsUnderRoot;
            public bool OnlyInjectWhenActive;
        }
    }
}

#endif
