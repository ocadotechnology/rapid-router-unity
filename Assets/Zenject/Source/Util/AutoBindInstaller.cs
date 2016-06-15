#if !ZEN_NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    // To use this, just add the ZenjectAutoBinding on to the monobehaviours in your scene
    // that you want to be automatically added to the container
    // Then also call Container.Install<AutoBindInstaller> from another installer
    public class AutoBindInstaller : Installer
    {
        readonly CompositionRoot _compRoot;

        public AutoBindInstaller(CompositionRoot compRoot)
        {
            Assert.That(!(compRoot is GlobalCompositionRoot),
                "You cannot use AutoBindInstaller from within a global installer");

            _compRoot = compRoot;
        }

        public override void InstallBindings()
        {
            foreach (var autoBinding in SceneCompositionRoot.GetSceneRootObjects(_compRoot.gameObject.scene, _compRoot.AllowInjectInactive)
                .SelectMany(x => x.GetComponentsInChildren<ZenjectAutoBinding>()))
            {
                if (autoBinding == null)
                {
                    continue;
                }

                var component = autoBinding.Component;
                var bindType = autoBinding.BindType;

                if (component == null)
                {
                    continue;
                }

                if (bindType == ZenjectAutoBinding.BindTypes.ToInstance
                    || bindType == ZenjectAutoBinding.BindTypes.ToInstanceAndInterfaces)
                {
                    Container.Bind(component.GetType()).ToInstance(component);
                }

                if (bindType == ZenjectAutoBinding.BindTypes.ToInterfaces
                    || bindType == ZenjectAutoBinding.BindTypes.ToInstanceAndInterfaces)
                {
                    Container.BindAllInterfacesToInstance(component);
                }
            }
        }
    }
}

#endif
