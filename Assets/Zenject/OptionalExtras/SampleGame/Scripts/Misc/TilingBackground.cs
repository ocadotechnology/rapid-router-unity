using UnityEngine;
using System.Collections;

namespace Asteroids
{
    public class TilingBackground : MonoBehaviour 
    {
        public float Speed;

        Vector2 offset;

        void Update()
        {
            offset.y += Speed * Time.deltaTime;
            GetComponent<Renderer>().material.mainTextureOffset = offset;
        }
    }
}
