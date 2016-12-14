using System;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    abstract public class BasePC2D : MonoBehaviour
    {
        public ProCamera2D ProCamera2D;

        protected Func<Vector3, float> Vector3H;
        protected Func<Vector3, float> Vector3V;
        protected Func<Vector3, float> Vector3D;
        protected Func<float, float, Vector3> VectorHV;
        protected Func<float, float, float, Vector3> VectorHVD;

        protected Transform _transform;

        bool _enabled;

        protected virtual void Awake()
        {
            _transform = transform;

            if (ProCamera2D == null && Camera.main != null)
                ProCamera2D = Camera.main.GetComponent<ProCamera2D>();
            else if (ProCamera2D == null)
                ProCamera2D = FindObjectOfType(typeof(ProCamera2D)) as ProCamera2D;
            
            if (ProCamera2D == null)
            {
                Debug.LogError(GetType().Name + ": ProCamera2D not set and not found on the MainCamera, or no camera with the MainCamera tag assigned.");
                return;
            }

            if(enabled)
                Enable();

            ResetAxisFunctions();
        }

        protected virtual void OnEnable()
        {
            Enable();
        }

        protected virtual void OnDisable()
        {
            Disable();
        }

        protected virtual void OnDestroy()
        {
            Disable();
        }

        /// <summary>Called when the method Reset is called on the Core. Use it to reset an extension.</summary>
        public virtual void OnReset()
        {   
        }

        void Enable()
        {
            if (_enabled)
                return;
            
            _enabled = true;
            ProCamera2D.OnReset += OnReset;
        }

        void Disable()
        {
            if (ProCamera2D != null && _enabled)
            {
                _enabled = false;
                ProCamera2D.OnReset -= OnReset;
            }
        }

        void ResetAxisFunctions()
        {
            if (Vector3H != null)
                return;

            switch (ProCamera2D.Axis)
            {
                case MovementAxis.XY:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.y;
                    Vector3D = vector => vector.z;
                    VectorHV = (h, v) => new Vector3(h, v, 0);
                    VectorHVD = (h, v, d) => new Vector3(h, v, d);
                    break;
                case MovementAxis.XZ:
                    Vector3H = vector => vector.x;
                    Vector3V = vector => vector.z;
                    Vector3D = vector => vector.y;
                    VectorHV = (h, v) => new Vector3(h, 0, v);
                    VectorHVD = (h, v, d) => new Vector3(h, d, v);
                    break;
                case MovementAxis.YZ:
                    Vector3H = vector => vector.z;
                    Vector3V = vector => vector.y;
                    Vector3D = vector => vector.x;
                    VectorHV = (h, v) => new Vector3(0, v, h);
                    VectorHVD = (h, v, d) => new Vector3(d, v, h);
                    break;
            } 
        }

        #if UNITY_EDITOR
        int _drawGizmosCounter;

        void OnDrawGizmos()
        {
            if (!enabled)
                return;

            if (ProCamera2D == null && Camera.main != null)
                ProCamera2D = Camera.main.GetComponent<ProCamera2D>();

            if (ProCamera2D == null)
                return;

            // Don't draw gizmos on other cameras
            if (Camera.current != ProCamera2D.GameCamera &&
                ((UnityEditor.SceneView.lastActiveSceneView != null && Camera.current != UnityEditor.SceneView.lastActiveSceneView.camera) ||
                (UnityEditor.SceneView.lastActiveSceneView == null)))
                return;

            ResetAxisFunctions();

            // HACK to prevent Unity bug on startup: http://forum.unity3d.com/threads/screen-position-out-of-view-frustum.9918/
            _drawGizmosCounter++;
            if (_drawGizmosCounter < 5 && UnityEditor.EditorApplication.timeSinceStartup < 60f)
                return;

            DrawGizmos();
        }

        protected virtual void DrawGizmos()
        {
        }
        #endif
    }
}