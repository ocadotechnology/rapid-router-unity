using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Zenject;

namespace Asteroids
{
    public class ShipHooks : MonoBehaviour
    {
        public event Action<Collider> TriggerEnter = delegate {};

        public MeshRenderer MeshRenderer;
        public ParticleEmitter ParticleEmitter;

        public void OnTriggerEnter(Collider other)
        {
            TriggerEnter(other);
        }
    }
}

