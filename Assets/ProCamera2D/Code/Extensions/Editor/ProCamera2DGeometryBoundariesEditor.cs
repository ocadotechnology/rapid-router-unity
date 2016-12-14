using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DGeometryBoundaries))]
    public class ProCamera2DGeometryBoundariesEditor : Editor
    {
        void OnEnable()
        {
            ProCamera2DEditorHelper.AssignProCamera2D(target as BasePC2D);
        }

        public override void OnInspectorGUI()
        {
            var proCamera2DGeometryBoundaries = (ProCamera2DGeometryBoundaries)target;

            if (proCamera2DGeometryBoundaries.ProCamera2D == null)
            {
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);
                return;
            }

            DrawDefaultInspector();

            if (proCamera2DGeometryBoundaries.ProCamera2D.CenterTargetOnStart)
                EditorGUILayout.HelpBox("Centering on target at start while using geometry boundaries might cause the camera to get stuck.", MessageType.Warning, true);
            
        }
    }
}