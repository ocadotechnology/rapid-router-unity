using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    #if UNITY_5_3_OR_NEWER
    [HelpURL("http://www.procamera2d.com/user-guide/extension-limit-distance/")]
    #endif
    public class ProCamera2DLimitDistance : BasePC2D, IPositionDeltaChanger
    {
        public static string ExtensionName = "Limit Distance";

        public bool LimitHorizontalCameraDistance = true;
        public float MaxHorizontalTargetDistance = .8f;

        public bool LimitVerticalCameraDistance = true;
        public float MaxVerticalTargetDistance = .8f;

        protected override void Awake()
        {
            base.Awake();

            ProCamera2D.Instance.AddPositionDeltaChanger(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ProCamera2D.RemovePositionDeltaChanger(this);
        }

        #region IPositionDeltaChanger implementation
        public Vector3 AdjustDelta(float deltaTime, Vector3 originalDelta)
        {
            if (!enabled)
                return originalDelta;

            
            var horizontalDeltaMovement = Vector3H(originalDelta);
            var horizontalExtra = false;
            if (LimitHorizontalCameraDistance)
            {
                var horizontalArea = (ProCamera2D.ScreenSizeInWorldCoordinates.x / 2) * MaxHorizontalTargetDistance;

                if (ProCamera2D.CameraTargetPosition.x > horizontalDeltaMovement + Vector3H(ProCamera2D.LocalPosition) + horizontalArea)
                {
                    horizontalDeltaMovement = ProCamera2D.CameraTargetPosition.x - (Vector3H(ProCamera2D.LocalPosition) + horizontalArea);
                    horizontalExtra = true;
                }
                else if (ProCamera2D.CameraTargetPosition.x < horizontalDeltaMovement + Vector3H(ProCamera2D.LocalPosition) - horizontalArea)
                {
                    horizontalDeltaMovement = ProCamera2D.CameraTargetPosition.x - (Vector3H(ProCamera2D.LocalPosition) - horizontalArea);
                    horizontalExtra = true;
                }
            }


            var verticalDeltaMovement = Vector3V(originalDelta);
            var verticalExtra = false;
            if (LimitVerticalCameraDistance)
            {
                var verticalArea = (ProCamera2D.ScreenSizeInWorldCoordinates.y / 2) * MaxVerticalTargetDistance;

                if (ProCamera2D.CameraTargetPosition.y > verticalDeltaMovement + Vector3V(ProCamera2D.LocalPosition) + verticalArea)
                {
                    verticalDeltaMovement = ProCamera2D.CameraTargetPosition.y - (Vector3V(ProCamera2D.LocalPosition) + verticalArea);
                    verticalExtra = true;
                }
                else if (ProCamera2D.CameraTargetPosition.y < verticalDeltaMovement + Vector3V(ProCamera2D.LocalPosition) - verticalArea)
                {
                    verticalDeltaMovement = ProCamera2D.CameraTargetPosition.y - (Vector3V(ProCamera2D.LocalPosition) - verticalArea);
                    verticalExtra = true;
                }
            }

            ProCamera2D.CameraTargetPositionSmoothed = new Vector2(
                horizontalExtra ? Vector3H(ProCamera2D.LocalPosition) + horizontalDeltaMovement : ProCamera2D.CameraTargetPositionSmoothed.x, 
                verticalExtra ? Vector3V(ProCamera2D.LocalPosition) + verticalDeltaMovement : ProCamera2D.CameraTargetPositionSmoothed.y);

            return VectorHV(horizontalDeltaMovement, verticalDeltaMovement);
        }

        public int PDCOrder { get { return _pdcOrder; } set { _pdcOrder = value; } }
        int _pdcOrder = 2000;
        #endregion

        #if UNITY_EDITOR
        protected override void DrawGizmos()
        {
            base.DrawGizmos();

            var gameCamera = ProCamera2D.GetComponent<Camera>();
            var cameraDimensions = gameCamera.orthographic ? Utils.GetScreenSizeInWorldCoords(gameCamera) : Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(transform.position)));
            float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

            // Limit cam distance
            if (LimitHorizontalCameraDistance)
            {
                Gizmos.color = EditorPrefsX.GetColor(PrefsData.CamDistanceColorKey, PrefsData.CamDistanceColorValue);
                Gizmos.DrawRay(VectorHVD(Vector3H(transform.position) + (cameraDimensions.x / 2) * MaxHorizontalTargetDistance, Vector3V(transform.position) - cameraDimensions.y / 2, cameraDepthOffset), transform.up * cameraDimensions.y);
                Gizmos.DrawRay(VectorHVD(Vector3H(transform.position) - (cameraDimensions.x / 2) * MaxHorizontalTargetDistance, Vector3V(transform.position) - cameraDimensions.y / 2, cameraDepthOffset), transform.up * cameraDimensions.y);
            }

            if (LimitVerticalCameraDistance)
            {
                Gizmos.color = EditorPrefsX.GetColor(PrefsData.CamDistanceColorKey, PrefsData.CamDistanceColorValue);
                Gizmos.DrawRay(VectorHVD(Vector3H(transform.position) - cameraDimensions.x / 2, Vector3V(transform.position) - (cameraDimensions.y / 2) * MaxVerticalTargetDistance, cameraDepthOffset), transform.right * cameraDimensions.x);
                Gizmos.DrawRay(VectorHVD(Vector3H(transform.position) - cameraDimensions.x / 2, Vector3V(transform.position) + (cameraDimensions.y / 2) * MaxVerticalTargetDistance, cameraDepthOffset), transform.right * cameraDimensions.x);
            }
        }
        #endif
    }
}