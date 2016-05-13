using System;
using UnityEngine;
using System.Collections;
using Zenject;

namespace Asteroids
{
    public enum ShipStates
    {
        Moving,
        Dead,
        WaitingToStart,
        Count,
    }

    public abstract class ShipState
    {
        protected Ship _ship;

        public ShipState(Ship ship)
        {
            _ship = ship;
        }

        public abstract void Update();

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            // do nothing
        }
    }
}
