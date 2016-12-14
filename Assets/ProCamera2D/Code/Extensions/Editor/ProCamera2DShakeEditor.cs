using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DShake))]
    public class ProCamera2DShakeEditor : Editor
    {
        MonoScript _script;
        GUIContent _tooltip;

        ReorderableList _shakePresetsList;

        static List<ShakePreset> _playModePresets = new List<ShakePreset>();
        static string _currentScene;

        void OnEnable()
        {
            ProCamera2DEditorHelper.AssignProCamera2D(target as BasePC2D);

            var proCamera2DShake = (ProCamera2DShake)target;

            _script = MonoScript.FromMonoBehaviour(proCamera2DShake);

            // Get presets from play mode
            if (_playModePresets == null)
                _playModePresets = new List<ShakePreset>();
            
            serializedObject.Update();

            #if UNITY_5_3_OR_NEWER
            if (_currentScene != UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name)
                _playModePresets.Clear();

            _currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
            #else
            if (_currentScene != EditorApplication.currentScene)
                _playModePresets.Clear();

            _currentScene = EditorApplication.currentScene;
            #endif

            if (!Application.isPlaying && _playModePresets.Count > 0)
            {
                var list = serializedObject.FindProperty("ShakePresets");
                list.ClearArray();
                for (int i = 0; i < _playModePresets.Count; i++)
                {
                    list.InsertArrayElementAtIndex(i);
                    var preset = list.GetArrayElementAtIndex(i);
                    preset.FindPropertyRelative("Name").stringValue = _playModePresets[i].Name;
                    preset.FindPropertyRelative("Strength").vector3Value = _playModePresets[i].Strength;
                    preset.FindPropertyRelative("Duration").floatValue = _playModePresets[i].Duration;
                    preset.FindPropertyRelative("Vibrato").intValue = _playModePresets[i].Vibrato;
                    preset.FindPropertyRelative("Smoothness").floatValue = _playModePresets[i].Smoothness;
                    preset.FindPropertyRelative("Randomness").floatValue = _playModePresets[i].Randomness;
                    preset.FindPropertyRelative("InitialAngle").floatValue = _playModePresets[i].InitialAngle;
                    preset.FindPropertyRelative("Rotation").vector3Value = _playModePresets[i].Rotation;
                    preset.FindPropertyRelative("IgnoreTimeScale").boolValue = _playModePresets[i].IgnoreTimeScale;
                }
                _playModePresets.Clear();
            }
            serializedObject.ApplyModifiedProperties();

            // Shake presets list
            _shakePresetsList = new ReorderableList(serializedObject, serializedObject.FindProperty("ShakePresets"), false, true, false, true);

            _shakePresetsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                var element = _shakePresetsList.serializedProperty.GetArrayElementAtIndex(index);

                // Name field
                EditorGUI.PropertyField(new Rect(
                        rect.x,
                        rect.y,
                        rect.width / 4f,
                        EditorGUIUtility.singleLineHeight * 1.1f),
                    element.FindPropertyRelative("Name"), GUIContent.none);
                   
                // Load button
                if (GUI.Button(new Rect(
                            rect.x + rect.width / 4f + 5,
                            rect.y,
                            rect.width / 4f - 5,
                            EditorGUIUtility.singleLineHeight * 1.1f), "Load"))
                {
                    proCamera2DShake.Strength = element.FindPropertyRelative("Strength").vector3Value;
                    proCamera2DShake.Duration = element.FindPropertyRelative("Duration").floatValue;
                    proCamera2DShake.Vibrato = element.FindPropertyRelative("Vibrato").intValue;
                    proCamera2DShake.Smoothness = element.FindPropertyRelative("Smoothness").floatValue;
                    proCamera2DShake.Randomness = element.FindPropertyRelative("Randomness").floatValue;
                    proCamera2DShake.InitialAngle = element.FindPropertyRelative("InitialAngle").floatValue;
                    proCamera2DShake.Rotation = element.FindPropertyRelative("Rotation").vector3Value;
                    proCamera2DShake.IgnoreTimeScale = element.FindPropertyRelative("IgnoreTimeScale").boolValue;

                    proCamera2DShake.UseRandomInitialAngle = proCamera2DShake.InitialAngle < 0;

                    EditorUtility.SetDirty(target);
                }

                // Save button
                if (GUI.Button(new Rect(
                            rect.x + 2 * rect.width / 4f + 5,
                            rect.y,
                            rect.width / 4f - 5,
                            EditorGUIUtility.singleLineHeight * 1.1f), "Save"))
                {
                    element.FindPropertyRelative("Strength").vector3Value = proCamera2DShake.Strength;
                    element.FindPropertyRelative("Duration").floatValue = proCamera2DShake.Duration;
                    element.FindPropertyRelative("Vibrato").intValue = proCamera2DShake.Vibrato;
                    element.FindPropertyRelative("Smoothness").floatValue = proCamera2DShake.Smoothness;
                    element.FindPropertyRelative("Randomness").floatValue = proCamera2DShake.Randomness;
                    element.FindPropertyRelative("InitialAngle").floatValue = proCamera2DShake.InitialAngle;
                    element.FindPropertyRelative("Rotation").vector3Value = proCamera2DShake.Rotation;
                    element.FindPropertyRelative("IgnoreTimeScale").boolValue = proCamera2DShake.IgnoreTimeScale;

                    proCamera2DShake.UseRandomInitialAngle = proCamera2DShake.InitialAngle < 0;

                    EditorUtility.SetDirty(target);

                    Repaint();
                }

                // Shake button
                GUI.enabled = Application.isPlaying;
                if (GUI.Button(new Rect(
                            rect.x + 3 * rect.width / 4f + 5,
                            rect.y,
                            rect.width / 4f - 5,
                            EditorGUIUtility.singleLineHeight * 1.1f), "Shake!"))
                {
                    proCamera2DShake.Shake(
                        element.FindPropertyRelative("Duration").floatValue,
                        element.FindPropertyRelative("Strength").vector3Value,
                        element.FindPropertyRelative("Vibrato").intValue,
                        element.FindPropertyRelative("Randomness").floatValue,
                        element.FindPropertyRelative("InitialAngle").floatValue,
                        element.FindPropertyRelative("Rotation").vector3Value,
                        element.FindPropertyRelative("Smoothness").floatValue,
                        element.FindPropertyRelative("IgnoreTimeScale").boolValue
                    );
                }
                GUI.enabled = true;
            };

            _shakePresetsList.drawHeaderCallback = (Rect rect) =>
            {  
                EditorGUI.LabelField(rect, "Shake Presets");
            };
            
            _shakePresetsList.elementHeight = 30;
            _shakePresetsList.draggable = true;
        }

        void OnDisable()
        {
            var proCamera2DShake = (ProCamera2DShake)target;

            _playModePresets = proCamera2DShake.ShakePresets;
        }

        public override void OnInspectorGUI()
        {
            var proCamera2DShake = (ProCamera2DShake)target;
            
            if (proCamera2DShake.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

            serializedObject.Update();

            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ProCamera2D"), _tooltip);            

            // Strength
            _tooltip = new GUIContent("Strength", "The shake strength on each axis");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Strength"), _tooltip);

            // Duration
            _tooltip = new GUIContent("Duration", "The duration of the shake");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Duration"), _tooltip);

            // Vibrato
            _tooltip = new GUIContent("Vibrato", "Indicates how much will the shake vibrate");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Vibrato"), _tooltip);

            // Smoothness
            _tooltip = new GUIContent("Smoothness", "Indicates how smooth the shake will be");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Smoothness"), _tooltip);

            // Randomness
            _tooltip = new GUIContent("Randomness", "Indicates how much random the shake will be");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Randomness"), _tooltip);

            // Random initial direction
            EditorGUILayout.BeginHorizontal();
            _tooltip = new GUIContent("Use Random Initial Angle", "If enabled, the initial shaking angle will be random");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseRandomInitialAngle"), _tooltip);

            if (!proCamera2DShake.UseRandomInitialAngle)
            {
                _tooltip = new GUIContent("Initial Angle", "");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("InitialAngle"), _tooltip);
            }
            EditorGUILayout.EndHorizontal();

            // Rotation
            _tooltip = new GUIContent("Rotation", "The maximum rotation the camera will reach");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Rotation"), _tooltip);

            // Ignore time scale
            _tooltip = new GUIContent("Ignore TimeScale", "If enabled, the shake will occur even if the timeScale is 0");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IgnoreTimeScale"), _tooltip);

            // Shake test buttons
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Shake!"))
            {
                proCamera2DShake.Shake();
            }
                
            if (GUILayout.Button("Stop!"))
            {
                proCamera2DShake.StopShaking();
            }
            GUI.enabled = true;

            // Add to presets button
            if (GUILayout.Button("Add To Presets"))
            {
                proCamera2DShake.ShakePresets.Add(new ShakePreset()
                    {
                        Name = "ShakePreset" + proCamera2DShake.ShakePresets.Count,
                        Strength = proCamera2DShake.Strength,
                        Duration = proCamera2DShake.Duration,
                        Vibrato = proCamera2DShake.Vibrato,
                        Randomness = proCamera2DShake.Randomness,
                        Smoothness = proCamera2DShake.Smoothness,
                        InitialAngle = proCamera2DShake.UseRandomInitialAngle ? -1f : proCamera2DShake.InitialAngle,
                        Rotation = proCamera2DShake.Rotation,
                        IgnoreTimeScale = proCamera2DShake.IgnoreTimeScale,
                    });
            }

            // Presets list
            EditorGUILayout.Space();
            _shakePresetsList.DoLayoutList();


            serializedObject.ApplyModifiedProperties();
        }
    }
}