using UnityEditor;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DLimitDistance))]
    public class ProCamera2DLimitDistanceEditor : Editor
    {
        GUIContent _tooltip;

        MonoScript _script;

        void OnEnable()
        {
            ProCamera2DEditorHelper.AssignProCamera2D(target as BasePC2D);

            _script = MonoScript.FromMonoBehaviour((ProCamera2DLimitDistance)target);
        }

        public override void OnInspectorGUI()
        {
            var proCamera2DLimitDistance = (ProCamera2DLimitDistance)target;

            serializedObject.Update();

            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ProCamera2D"), _tooltip);

            if(proCamera2DLimitDistance.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

            // Limit horizontal
            EditorGUILayout.BeginHorizontal();

            _tooltip = new GUIContent("Limit Horizontal Distance", "Prevent the camera target from getting out of the screeen. Use this if you have a high follow smoothness and your targets are getting out of the screen.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LimitHorizontalCameraDistance"), _tooltip);

            if (proCamera2DLimitDistance.LimitHorizontalCameraDistance)
            {
                _tooltip = new GUIContent(" ", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxHorizontalTargetDistance"), _tooltip);
            }

            EditorGUILayout.EndHorizontal();

            // Max speed vertical
            EditorGUILayout.BeginHorizontal();

            _tooltip = new GUIContent("Limit Vertical Distance", "Prevent the camera target from getting out of the screen. Use this if you have a high follow smoothness and your targets are getting out of the screen.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LimitVerticalCameraDistance"), _tooltip);

            if (proCamera2DLimitDistance.LimitVerticalCameraDistance)
            {
                _tooltip = new GUIContent(" ", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxVerticalTargetDistance"), _tooltip);
            }

            EditorGUILayout.EndHorizontal();

            // Limit values
            if (proCamera2DLimitDistance.MaxHorizontalTargetDistance < .1f)
                proCamera2DLimitDistance.MaxHorizontalTargetDistance = .1f;

            if (proCamera2DLimitDistance.MaxHorizontalTargetDistance > 1f)
                proCamera2DLimitDistance.MaxHorizontalTargetDistance = 1f;

            if (proCamera2DLimitDistance.MaxVerticalTargetDistance < .1f)
                proCamera2DLimitDistance.MaxVerticalTargetDistance = .1f;

            if (proCamera2DLimitDistance.MaxVerticalTargetDistance > 1f)
                proCamera2DLimitDistance.MaxVerticalTargetDistance = 1f;

            serializedObject.ApplyModifiedProperties();
        }
    }
}