using System;
using UnityEngine;
using System.Collections;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    #if UNITY_5_3_OR_NEWER
    [HelpURL("http://www.procamera2d.com/user-guide/extension-zoom-to-fit/")]
    #endif
    public class ProCamera2DZoomToFitTargets : BasePC2D, ISizeOverrider
    {
        public static string ExtensionName = "Zoom To Fit";

        public float ZoomOutBorder = .6f;
        public float ZoomInBorder = .4f;
        public float ZoomInSmoothness = 2f;
        public float ZoomOutSmoothness = 1f;

        public float MaxZoomInAmount = 2f;
        public float MaxZoomOutAmount = 4f;

        public bool DisableWhenOneTarget = true;

        float _zoomVelocity;

        float _initialCamSize;
        float _previousCamSize;
        float _targetCamSize;
        float _targetCamSizeSmoothed;

        float _minCameraSize;
        float _maxCameraSize;

        override protected void Awake()
        {
            base.Awake();

            if (ProCamera2D == null)
                return;
            
            ProCamera2D.AddSizeOverrider(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ProCamera2D.RemoveSizeOverrider(this);
        }

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            _initialCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;
            _targetCamSize = _initialCamSize;
            _targetCamSizeSmoothed = _targetCamSize;
        }

        #region ISizeOverrider implementation

        public float OverrideSize(float deltaTime, float originalSize)
        {
            if (!enabled)
                return originalSize;

            _targetCamSizeSmoothed = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;

            if (DisableWhenOneTarget && ProCamera2D.CameraTargets.Count <= 1)
                _targetCamSize = _initialCamSize;
            else
            {
                if (_previousCamSize == ProCamera2D.ScreenSizeInWorldCoordinates.y)
                {
                    _targetCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y * .5f;
                    _targetCamSizeSmoothed = _targetCamSize;
                    _zoomVelocity = 0f;
                }

                UpdateTargetCamSize();
            }

            _previousCamSize = ProCamera2D.ScreenSizeInWorldCoordinates.y;

            return _targetCamSizeSmoothed = Mathf.SmoothDamp(_targetCamSizeSmoothed, _targetCamSize, ref _zoomVelocity, _targetCamSize < _targetCamSizeSmoothed ? ZoomInSmoothness : ZoomOutSmoothness);
        }

        public int SOOrder { get { return _soOrder; } set { _soOrder = value; } }
        int _soOrder = 0;

        #endregion

        override public void OnReset()
        {
            _zoomVelocity = 0;

            _previousCamSize = _initialCamSize;
            _targetCamSize = _initialCamSize;
            _targetCamSizeSmoothed = _initialCamSize;
        }

        void UpdateTargetCamSize()
        {
            // Targets bounding box
            float maxX = Mathf.NegativeInfinity;
            float minX = Mathf.Infinity;
            float maxY = Mathf.NegativeInfinity;
            float minY = Mathf.Infinity;
            for (int i = 0; i < ProCamera2D.CameraTargets.Count; i++)
            {
                var targetPos = new Vector2(Vector3H(ProCamera2D.CameraTargets[i].TargetPosition) + ProCamera2D.CameraTargets[i].TargetOffset.x, Vector3V(ProCamera2D.CameraTargets[i].TargetPosition) + ProCamera2D.CameraTargets[i].TargetOffset.y);

                maxX = targetPos.x > maxX ? targetPos.x : maxX;
                minX = targetPos.x < minX ? targetPos.x : minX;
                maxY = targetPos.y > maxY ? targetPos.y : maxY;
                minY = targetPos.y < minY ? targetPos.y : minY;
            }

            var distanceMaxX = Mathf.Abs(maxX - minX) * .5f;
            var distanceMaxY = Mathf.Abs(maxY - minY) * .5f;

            // Zoom out
            if (distanceMaxX > ProCamera2D.ScreenSizeInWorldCoordinates.x * ZoomOutBorder * .5f ||
                distanceMaxY > ProCamera2D.ScreenSizeInWorldCoordinates.y * ZoomOutBorder * .5f)
            {
                if (distanceMaxX / ProCamera2D.ScreenSizeInWorldCoordinates.x >= distanceMaxY / ProCamera2D.ScreenSizeInWorldCoordinates.y)
                    _targetCamSize = (distanceMaxX / ProCamera2D.GameCamera.aspect) / ZoomOutBorder;
                else
                    _targetCamSize = distanceMaxY / ZoomOutBorder;
            }
            // Zoom in
            else if (distanceMaxX < ProCamera2D.ScreenSizeInWorldCoordinates.x * ZoomInBorder * .5f &&
                     distanceMaxY < ProCamera2D.ScreenSizeInWorldCoordinates.y * ZoomInBorder * .5f)
            {
                if (distanceMaxX / ProCamera2D.ScreenSizeInWorldCoordinates.x >= distanceMaxY / ProCamera2D.ScreenSizeInWorldCoordinates.y)
                    _targetCamSize = (distanceMaxX / ProCamera2D.GameCamera.aspect) / ZoomInBorder;
                else
                    _targetCamSize = distanceMaxY / ZoomInBorder;
            }

            _minCameraSize = _initialCamSize / MaxZoomInAmount;
            _maxCameraSize = _initialCamSize * MaxZoomOutAmount;
            _targetCamSize = Mathf.Clamp(_targetCamSize, _minCameraSize, _maxCameraSize);
        }

        #if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            base.DrawGizmos();

            var gameCamera = ProCamera2D.GetComponent<Camera>();
            var cameraDimensions = gameCamera.orthographic ? Utils.GetScreenSizeInWorldCoords(gameCamera) : Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(transform.localPosition)));
            float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);
            var cameraCenter = VectorHVD(Vector3H(transform.position), Vector3V(transform.position), cameraDepthOffset);

            var bordersColor = DisableWhenOneTarget && ProCamera2D.CameraTargets.Count <= 1 ? EditorPrefsX.GetColor(PrefsData.ZoomToFitColorKey, Color.white) * .75f : EditorPrefsX.GetColor(PrefsData.ZoomToFitColorKey, Color.white);
            
            // Zoom out border
            Gizmos.color = Math.Abs(_targetCamSizeSmoothed - _maxCameraSize) < .01f ? bordersColor * .5f : bordersColor;
            Gizmos.DrawWireCube(cameraCenter, VectorHV(cameraDimensions.x, cameraDimensions.y) * ZoomOutBorder);
            Utils.DrawArrowForGizmo(cameraCenter + VectorHV(cameraDimensions.x / 2 * ZoomOutBorder, cameraDimensions.y / 2 * ZoomOutBorder), VectorHV(.05f * cameraDimensions.y, .05f * cameraDimensions.y), .04f * cameraDimensions.y);
            Utils.DrawArrowForGizmo(cameraCenter - VectorHV(cameraDimensions.x / 2 * ZoomOutBorder, cameraDimensions.y / 2 * ZoomOutBorder), VectorHV(-.05f * cameraDimensions.y, -.05f * cameraDimensions.y), .04f * cameraDimensions.y);

            // Zoom in border
            Gizmos.color = Math.Abs(_targetCamSizeSmoothed - _minCameraSize) < .01f ? bordersColor * .75f : bordersColor;
            Gizmos.DrawWireCube(cameraCenter, VectorHV(cameraDimensions.x, cameraDimensions.y) * ZoomInBorder);
            Utils.DrawArrowForGizmo(cameraCenter + VectorHV(cameraDimensions.x / 2 * ZoomInBorder, cameraDimensions.y / 2 * ZoomInBorder), VectorHV(-.05f * cameraDimensions.y, -.05f * cameraDimensions.y), .04f * cameraDimensions.y);
            Utils.DrawArrowForGizmo(cameraCenter - VectorHV(cameraDimensions.x / 2 * ZoomInBorder, cameraDimensions.y / 2 * ZoomInBorder), VectorHV(.05f * cameraDimensions.y, .05f * cameraDimensions.y), .04f * cameraDimensions.y);
        }
        #endif
    }
}