#if !ZEN_NOT_UNITY3D

#pragma warning disable 414
using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;
using UnityEngine;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace Zenject
{
    public abstract class CompositionRoot : MonoBehaviour
    {
        bool _isDisposed;

        public abstract DiContainer Container
        {
            get;
        }

        public abstract IFacade RootFacade
        {
            get;
        }

        public abstract bool AllowInjectInactive
        {
            get;
        }

        public void Update()
        {
            // Avoid spamming the log if RootFacade failed to initialize
            if (RootFacade != null)
            {
                RootFacade.Tick();
            }
        }

        public void FixedUpdate()
        {
            // Avoid spamming the log if RootFacade failed to initialize
            if (RootFacade != null)
            {
                RootFacade.FixedTick();
            }
        }

        public void LateUpdate()
        {
            // Avoid spamming the log if RootFacade failed to initialize
            if (RootFacade != null)
            {
                RootFacade.LateTick();
            }
        }

        public void OnApplicationQuit()
        {
            // RootFacade is null if the SceneCompositionRoot is not enabled then the user quits
            if (RootFacade != null)
            {
                // In some cases we have monobehaviour's that are bound to IDisposable, and who have
                // also been set with Application.DontDestroyOnLoad so that the Dispose() is always
                // called instead of OnDestroy.  This is nice because we can actually reliably predict the
                // order Dispose() is called in which is not the case for OnDestroy.
                // However, when the user quits the app, OnDestroy is called even for objects that
                // have been marked with Application.DontDestroyOnLoad, and so the destruction order
                // changes.  So to address this case, dispose before the OnDestroy event below (OnApplicationQuit
                // is always called before OnDestroy) and then don't call dispose in OnDestroy
                Assert.That(!_isDisposed);
                RootFacade.Dispose();
                _isDisposed = true;
            }
        }

        public void OnDestroy()
        {
            // RootFacade is null if the SceneCompositionRoot is not enabled then the user quits
            if (RootFacade != null)
            {
                // See comment in OnApplicationQuit
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    RootFacade.Dispose();
                }
            }
        }
    }
}

#endif

