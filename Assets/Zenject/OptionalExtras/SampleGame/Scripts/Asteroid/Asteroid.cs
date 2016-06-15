using System;
using UnityEngine;
using System.Collections;
using Zenject;
using ModestTree;

namespace Asteroids
{
    public class Asteroid : MonoBehaviour
    {
        [Inject]
        LevelHelper _level = null;

        [Inject]
        Settings _settings = null;

        bool _hasDisposed;

        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public float Mass
        {
            get
            {
                return GetComponent<Rigidbody>().mass;
            }
            set
            {
                GetComponent<Rigidbody>().mass = value;
            }
        }

        public float Scale
        {
            get
            {
                var scale = transform.localScale;
                // We assume scale is uniform
                Assert.That(scale[0] == scale[1] && scale[1] == scale[2]);

                return scale[0];
            }
            set
            {
                transform.localScale = new Vector3(value, value, value);
                GetComponent<Rigidbody>().mass = value;
            }
        }

        public Vector3 Velocity
        {
            get
            {
                return GetComponent<Rigidbody>().velocity;
            }
            set
            {
                GetComponent<Rigidbody>().velocity = value;
            }
        }

        public void FixedTick()
        {
            // Limit speed to a maximum
            var speed = GetComponent<Rigidbody>().velocity.magnitude;

            if (speed > _settings.maxSpeed)
            {
                var dir = GetComponent<Rigidbody>().velocity / speed;
                GetComponent<Rigidbody>().velocity = dir * _settings.maxSpeed;
            }
        }

        public void Tick()
        {
            CheckForTeleport();
        }

        public void Dispose()
        {
            Assert.That(!_hasDisposed);
            _hasDisposed = true;
            GameObject.Destroy(gameObject);
        }

        void CheckForTeleport()
        {
            if (Position.x > _level.Right + Scale && IsMovingInDirection(Vector3.right))
            {
                transform.SetX(_level.Left - Scale);
            }
            else if (Position.x < _level.Left - Scale && IsMovingInDirection(-Vector3.right))
            {
                transform.SetX(_level.Right + Scale);
            }
            else if (Position.y < _level.Bottom - Scale && IsMovingInDirection(-Vector3.up))
            {
                transform.SetY(_level.Top + Scale);
            }
            else if (Position.y > _level.Top + Scale && IsMovingInDirection(Vector3.up))
            {
                transform.SetY(_level.Bottom - Scale);
            }
            transform.RotateAround(transform.position, Vector3.up, 30 * Time.deltaTime);
        }

        bool IsMovingInDirection(Vector3 dir)
        {
            return Vector3.Dot(dir, GetComponent<Rigidbody>().velocity) > 0;
        }

        [Serializable]
        public class Settings
        {
            public float massScaleFactor;
            public float maxSpeed;
        }

        public class Factory : GameObjectFactory<Asteroid>
        {
        }
    }
}
