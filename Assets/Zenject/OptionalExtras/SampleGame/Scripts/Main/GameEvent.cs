using System;
using UnityEngine;
using System.Collections;
using Zenject;

namespace Asteroids
{
    public class GameEvent
    {
        public static Action ShipCrashed = delegate { };
    }
}

