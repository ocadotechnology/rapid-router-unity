using UnityEngine;
using System.Collections;
using Zenject;
using ModestTree;

namespace Asteroids
{
    // This scene is an example of how decorators work
    // We override move settings to make the ship much slower and add a hotkey to manually spawn asteroids
    // Decorators are really useful in particular for running different test configurations
    public class AsteroidsDecoratorInstaller : DecoratorInstaller
    {
        public ShipStateMoving.Settings OverrideMoveSettings;

        // If you are injecting into an installer then you will need to put the binding in PreInstall
        public override void PreInstallBindings()
        {
            Container.Bind<ITickable>().ToSingle<TestHotKeysAdder>();
            // Do not spawn asteroids automatically
            Container.Bind<bool>().ToInstance(false).WhenInjectedInto<AsteroidManager>();
        }

        public override void PostInstallBindings()
        {
            // Rebinds should occur as a post-install binding so that they have a chance to override
            Container.Rebind<ShipStateMoving.Settings>().ToSingleInstance(OverrideMoveSettings);
        }
    }

    public class TestHotKeysAdder : ITickable
    {
        readonly AsteroidManager _asteroidManager;

        public TestHotKeysAdder(AsteroidManager asteroidManager)
        {
            _asteroidManager = asteroidManager;
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                _asteroidManager.SpawnNext();
                Log.Info("Spawned new asteroid!");
            }
        }
    }
}
