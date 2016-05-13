using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Zenject;
using ModestTree;

namespace Asteroids
{
    public class ShipStateFactory
    {
        DiContainer _container;

        public ShipStateFactory(DiContainer container)
        {
            _container = container;
        }

        public ShipState Create(ShipStates state, params object[] constructorArgs)
        {
            switch (state)
            {
                case ShipStates.Dead:
                    return _container.Instantiate<ShipStateDead>(constructorArgs);

                case ShipStates.Moving:
                    return _container.Instantiate<ShipStateMoving>(constructorArgs);

                case ShipStates.WaitingToStart:
                    return _container.Instantiate<ShipStateWaitingToStart>(constructorArgs);
            }

            Assert.That(false);
            return null;
        }
    }
}
