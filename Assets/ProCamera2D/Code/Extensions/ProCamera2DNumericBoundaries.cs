using UnityEngine;
using System;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    public struct NumericBoundariesSettings
    {
        public bool UseNumericBoundaries;
        public bool UseTopBoundary;
        public float TopBoundary;
        public bool UseBottomBoundary;
        public float BottomBoundary;
        public bool UseLeftBoundary;
        public float LeftBoundary;
        public bool UseRightBoundary;
        public float RightBoundary;
    }

    #if UNITY_5_3_OR_NEWER
    [HelpURL("http://www.procamera2d.com/user-guide/extension-numeric-boundaries/")]
    #endif
    public class ProCamera2DNumericBoundaries : BasePC2D, IPositionDeltaChanger, ISizeOverrider
    {
        public static string ExtensionName = "Numeric Boundaries";

        public NumericBoundariesSettings Settings
        {
            get
            {
                return new NumericBoundariesSettings()
                { 
                    UseNumericBoundaries = UseNumericBoundaries,
                    UseTopBoundary = UseTopBoundary,
                    TopBoundary = TopBoundary,
                    UseBottomBoundary = UseBottomBoundary,
                    BottomBoundary = BottomBoundary,
                    UseLeftBoundary = UseLeftBoundary,
                    LeftBoundary = LeftBoundary,
                    UseRightBoundary = UseRightBoundary,
                    RightBoundary = RightBoundary
                };
            }
            set
            {
                UseNumericBoundaries = value.UseNumericBoundaries;
                UseTopBoundary = value.UseTopBoundary;
                TopBoundary = value.TopBoundary;
                UseBottomBoundary = value.UseBottomBoundary;
                BottomBoundary = value.BottomBoundary;
                UseLeftBoundary = value.UseLeftBoundary;
                LeftBoundary = value.LeftBoundary;
                UseRightBoundary = value.UseRightBoundary;
                RightBoundary = value.RightBoundary;
            }
        }

        public Action OnBoundariesTransitionStarted;
        public Action OnBoundariesTransitionFinished;

        public bool UseNumericBoundaries = true;
        public bool UseTopBoundary;
        public float TopBoundary = 10f;
        public float TargetTopBoundary;

        public bool UseBottomBoundary = true;
        public float BottomBoundary = -10f;
        public float TargetBottomBoundary;

        public bool UseLeftBoundary;
        public float LeftBoundary = -10f;
        public float TargetLeftBoundary;

        public bool UseRightBoundary;
        public float RightBoundary = 10f;
        public float TargetRightBoundary;

        public bool IsCameraPositionHorizontallyBounded;
        public bool IsCameraPositionVerticallyBounded;

        public Coroutine TopBoundaryAnimRoutine;
        public Coroutine BottomBoundaryAnimRoutine;
        public Coroutine LeftBoundaryAnimRoutine;
        public Coroutine RightBoundaryAnimRoutine;
        public ProCamera2DTriggerBoundaries CurrentBoundariesTrigger;

        public Coroutine MoveCameraToTargetRoutine;

        public bool HasFiredTransitionStarted;
        public bool HasFiredTransitionFinished;

        public bool UseElasticBoundaries;

        [Range(0, 10f)]
        public float HorizontalElasticityDuration = .5f;
        public float HorizontalElasticitySize = 2f;

        [Range(0, 10f)]
        public float VerticalElasticityDuration = .5f;
        public float VerticalElasticitySize = 2f;

        public EaseType ElasticityEaseType = EaseType.EaseInOut;

        float _verticallyBoundedDuration;
        float _horizontallyBoundedDuration;

        protected override void Awake()
        {
            base.Awake();

            ProCamera2D.AddPositionDeltaChanger(this);
            ProCamera2D.AddSizeOverrider(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ProCamera2D.RemovePositionDeltaChanger(this);
            ProCamera2D.RemoveSizeOverrider(this);
        }

        #region IPositionDeltaChanger implementation

        public Vector3 AdjustDelta(float deltaTime, Vector3 originalDelta)
        {
            if (!enabled || !UseNumericBoundaries)
                return originalDelta;

            // Check movement in the horizontal dir
            IsCameraPositionHorizontallyBounded = false;
            ProCamera2D.IsCameraPositionLeftBounded = false;
            ProCamera2D.IsCameraPositionRightBounded = false;
            IsCameraPositionVerticallyBounded = false;
            ProCamera2D.IsCameraPositionTopBounded = false;
            ProCamera2D.IsCameraPositionBottomBounded = false;
            var newPosH = Vector3H(ProCamera2D.LocalPosition) + Vector3H(originalDelta);
            if (UseLeftBoundary && newPosH - ProCamera2D.ScreenSizeInWorldCoordinates.x / 2 < LeftBoundary)
            {
                newPosH = LeftBoundary + ProCamera2D.ScreenSizeInWorldCoordinates.x / 2;
                IsCameraPositionHorizontallyBounded = true;
                ProCamera2D.IsCameraPositionLeftBounded = true;
            }
            else if (UseRightBoundary && newPosH + ProCamera2D.ScreenSizeInWorldCoordinates.x / 2 > RightBoundary)
            {
                newPosH = RightBoundary - ProCamera2D.ScreenSizeInWorldCoordinates.x / 2;
                IsCameraPositionHorizontallyBounded = true;
                ProCamera2D.IsCameraPositionRightBounded = true;
            }

            // Check movement in the vertical dir
            var newPosV = Vector3V(ProCamera2D.LocalPosition) + Vector3V(originalDelta);;
            if (UseBottomBoundary && newPosV - ProCamera2D.ScreenSizeInWorldCoordinates.y / 2 < BottomBoundary)
            {
                newPosV = BottomBoundary + ProCamera2D.ScreenSizeInWorldCoordinates.y / 2;
                IsCameraPositionVerticallyBounded = true;
                ProCamera2D.IsCameraPositionBottomBounded = true;
            }
            else if (UseTopBoundary && newPosV + ProCamera2D.ScreenSizeInWorldCoordinates.y / 2 > TopBoundary)
            {
                newPosV = TopBoundary - ProCamera2D.ScreenSizeInWorldCoordinates.y / 2;
                IsCameraPositionVerticallyBounded = true;
                ProCamera2D.IsCameraPositionTopBounded = true;
            }

            // Elastic Boundaries
            if (UseElasticBoundaries)
            {
                // Horizontal
                if (IsCameraPositionHorizontallyBounded)
                {
                    _horizontallyBoundedDuration = Mathf.Min(HorizontalElasticityDuration, _horizontallyBoundedDuration + deltaTime);

                    var perc = 1f;
                    if (HorizontalElasticityDuration > 0)
                        perc = _horizontallyBoundedDuration / HorizontalElasticityDuration;

                    if (ProCamera2D.IsCameraPositionLeftBounded)
                    {
                        newPosH = Utils.EaseFromTo(
                            Mathf.Max(
                                LeftBoundary + ProCamera2D.ScreenSizeInWorldCoordinates.x / 2 - HorizontalElasticitySize, 
                                Vector3H(_transform.localPosition)), 
                            LeftBoundary + ProCamera2D.ScreenSizeInWorldCoordinates.x / 2, 
                            perc,
                            ElasticityEaseType);
                    }
                    else
                    {
                        newPosH = Utils.EaseFromTo(
                            Mathf.Min(
                                RightBoundary - ProCamera2D.ScreenSizeInWorldCoordinates.x / 2 + HorizontalElasticitySize, 
                                Vector3H(_transform.localPosition)), 
                            RightBoundary - ProCamera2D.ScreenSizeInWorldCoordinates.x / 2, 
                            perc,
                            ElasticityEaseType);
                    }
                }
                else
                {
                    _horizontallyBoundedDuration = Mathf.Max(0, _horizontallyBoundedDuration - deltaTime);
                }

                // Vertical
                if (IsCameraPositionVerticallyBounded)
                {
                    _verticallyBoundedDuration = Mathf.Min(VerticalElasticityDuration, _verticallyBoundedDuration + deltaTime);

                    var perc = 1f;
                    if (VerticalElasticityDuration > 0)
                        perc = _verticallyBoundedDuration / VerticalElasticityDuration;

                    if (ProCamera2D.IsCameraPositionBottomBounded)
                    {
                        newPosV = Utils.EaseFromTo(
                            Mathf.Max(
                                BottomBoundary + ProCamera2D.ScreenSizeInWorldCoordinates.y / 2 - VerticalElasticitySize, 
                                Vector3V(_transform.localPosition)), 
                            BottomBoundary + ProCamera2D.ScreenSizeInWorldCoordinates.y / 2, 
                            perc,
                            ElasticityEaseType);
                    }
                    else
                    {
                        newPosV = Utils.EaseFromTo(
                            Mathf.Min(
                                TopBoundary - ProCamera2D.ScreenSizeInWorldCoordinates.y / 2 + VerticalElasticitySize, 
                                Vector3V(_transform.localPosition)), 
                            TopBoundary - ProCamera2D.ScreenSizeInWorldCoordinates.y / 2, 
                            perc,
                            ElasticityEaseType);
                    }
                }
                else
                {
                    _verticallyBoundedDuration = Mathf.Max(0, _verticallyBoundedDuration - deltaTime);
                }
            }

            // Return the new delta
            return VectorHV(newPosH - Vector3H(ProCamera2D.LocalPosition), newPosV - Vector3V(ProCamera2D.LocalPosition));
        }

        public int PDCOrder { get { return _pdcOrder; } set { _pdcOrder = value; } }
        int _pdcOrder = 4000;

        #endregion

        #region ISizeOverrider implementation

        public float OverrideSize(float deltaTime, float originalSize)
        {
            if (!UseNumericBoundaries)
                return originalSize;

            var newSize = originalSize;

            // Set new size if outside boundaries
            var cameraMaxSize = new Vector2(RightBoundary - LeftBoundary, TopBoundary - BottomBoundary);
            if (UseRightBoundary && UseLeftBoundary && originalSize * ProCamera2D.GameCamera.aspect * 2f > cameraMaxSize.x)
            {
                newSize = cameraMaxSize.x / ProCamera2D.GameCamera.aspect / 2f;
            }

            if (UseTopBoundary && UseBottomBoundary && newSize * 2f > cameraMaxSize.y)
            {
                newSize = cameraMaxSize.y / 2f;
            }

            return newSize;
        }

        public int SOOrder { get { return _soOrder; } set { _soOrder = value; } }
        int _soOrder = 2000;

        #endregion

        #if UNITY_EDITOR
        override protected void DrawGizmos()
        {
            base.DrawGizmos();

            var gameCamera = ProCamera2D.GetComponent<Camera>();
            var cameraDimensions = gameCamera.orthographic ? Utils.GetScreenSizeInWorldCoords(gameCamera) : Utils.GetScreenSizeInWorldCoords(gameCamera, Mathf.Abs(Vector3D(transform.localPosition)));
            float cameraDepthOffset = Vector3D(ProCamera2D.transform.localPosition) + Mathf.Abs(Vector3D(transform.localPosition)) * Vector3D(ProCamera2D.transform.forward);

            Gizmos.color = EditorPrefsX.GetColor(PrefsData.NumericBoundariesColorKey, PrefsData.NumericBoundariesColorValue);

            if (UseNumericBoundaries)
            {
                if (UseTopBoundary)
                    Gizmos.DrawRay(VectorHVD(Vector3H(transform.localPosition) - cameraDimensions.x / 2, TopBoundary, cameraDepthOffset), transform.right * cameraDimensions.x);

                if (UseBottomBoundary)
                    Gizmos.DrawRay(VectorHVD(Vector3H(transform.localPosition) - cameraDimensions.x / 2, BottomBoundary, cameraDepthOffset), transform.right * cameraDimensions.x);

                if (UseRightBoundary)
                    Gizmos.DrawRay(VectorHVD(RightBoundary, Vector3V(transform.localPosition) - cameraDimensions.y / 2, cameraDepthOffset), transform.up * cameraDimensions.y);

                if (UseLeftBoundary)
                    Gizmos.DrawRay(VectorHVD(LeftBoundary, Vector3V(transform.localPosition) - cameraDimensions.y / 2, cameraDepthOffset), transform.up * cameraDimensions.y);
            }
        }
        #endif
    }
}