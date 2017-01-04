using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Com.LuisPedroFonseca.ProCamera2D
{
    [CustomEditor(typeof(ProCamera2DParallax))]
    public class ProCamera2DParallaxEditor : Editor
    {
        GUIContent _tooltip;

        ReorderableList _parallaxLayersList;

        ProCamera2D _proCamera2D;
        ProCamera2DParallax _proCamera2DParallax;

        MonoScript _script;

        void OnEnable()
        {
            ProCamera2DEditorHelper.AssignProCamera2D(target as BasePC2D);

            _proCamera2DParallax = (ProCamera2DParallax)target;

            _proCamera2D = _proCamera2DParallax.ProCamera2D;

            _script = MonoScript.FromMonoBehaviour(_proCamera2DParallax);

            // Parallax layers List
            _parallaxLayersList = new ReorderableList(serializedObject, serializedObject.FindProperty("ParallaxLayers"), false, true, true, true);

            // Draw element callback
            _parallaxLayersList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                var element = _parallaxLayersList.serializedProperty.GetArrayElementAtIndex(index);

                #if UNITY_5
                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y, 65, 10), new GUIContent("Camera", "The parallax camera"), EditorStyles.boldLabel);
                #else
				EditorGUI.PrefixLabel(new Rect(rect.x, rect.y, 65, 10), new GUIContent("Camera", "The parallax camera"));
                #endif
                EditorGUI.PropertyField(new Rect(
                        rect.x + 65,
                        rect.y,
                        80,
                        EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("ParallaxCamera"), GUIContent.none);


                // Speed slider
                #if UNITY_5
                EditorGUI.PrefixLabel(new Rect(rect.x + 170, rect.y, 65, 10), new GUIContent("Speed", "The relative speed at which the camera should move in comparison to the main camera."), EditorStyles.boldLabel);
                #else
				EditorGUI.PrefixLabel(new Rect(rect.x + 170, rect.y, 65, 10), new GUIContent("Speed", "The relative speed at which the camera should move in comparison to the main camera."));
                #endif
                EditorGUI.PropertyField(new Rect(
                        rect.x + 210,
                        rect.y,
                        rect.width - 210,
                        EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Speed"), GUIContent.none);
                

                // Layer mask
                #if UNITY_5
                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y + 25, 65, 10), new GUIContent("Culling Mask", "Which layers should this camera render?"), EditorStyles.boldLabel);
                #else
                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y + 25, 65, 10), new GUIContent("Culling Mask", "Which layers should this camera render?"));
                #endif
                EditorGUI.PropertyField(new Rect(
                        rect.x + 85,
                        rect.y + 25,
                        rect.width - 85,
                        EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("LayerMask"), GUIContent.none);
            };

            // Draw header callback
            _parallaxLayersList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Parallax layers");

            // Add element callback
            _parallaxLayersList.onAddCallback = list => AddParallaxLayer();

            // Remove element callback
            _parallaxLayersList.onRemoveCallback = list =>
            {  
                if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete this layer?", "Yes", "No"))
                {
                    var cam = list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("ParallaxCamera").objectReferenceValue as Camera;
                    if (cam != null)
                        DestroyImmediate(cam.gameObject);

                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };

            // Select element callback
            _parallaxLayersList.onSelectCallback = list =>
            {  
                EditorGUIUtility.PingObject(list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("ParallaxCamera").objectReferenceValue as Camera);
            };

            _parallaxLayersList.onReorderCallback = list =>
            {
                if (_proCamera2DParallax.ParallaxLayers[0].ParallaxCamera.clearFlags != CameraClearFlags.SolidColor &&
                    _proCamera2DParallax.ParallaxLayers[0].ParallaxCamera.clearFlags != CameraClearFlags.Skybox)
                    _proCamera2DParallax.ParallaxLayers[0].ParallaxCamera.clearFlags = CameraClearFlags.SolidColor;
            };

            _parallaxLayersList.elementHeight = 65;
            _parallaxLayersList.headerHeight = 18;
            _parallaxLayersList.draggable = true;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Show script link
            GUI.enabled = false;
            _script = EditorGUILayout.ObjectField("Script", _script, typeof(MonoScript), false) as MonoScript;
            GUI.enabled = true;

            // ProCamera2D
            _tooltip = new GUIContent("Pro Camera 2D", "");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ProCamera2D"), _tooltip);

            if (_proCamera2DParallax.ProCamera2D == null)
                EditorGUILayout.HelpBox("ProCamera2D is not set.", MessageType.Error, true);

            _proCamera2D = _proCamera2DParallax.ProCamera2D;
            if (_proCamera2D == null)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }

            // Perspective camera
            if (!_proCamera2D.GameCamera.orthographic)
                EditorGUILayout.HelpBox("ProCamera2D Parallax needs an orthographic projection camera to work.", MessageType.Error, true);

            // Parallax layers List
            _parallaxLayersList.DoLayoutList();

            var areSpeedsNotOrdered = false;
            var isLayerMaskEmpty = false;
            var isLayerMaskOverlapping = false;
            for (int i = 0; i < _proCamera2DParallax.ParallaxLayers.Count; i++)
            {
                // Detect unordered speeds
                if (i < _proCamera2DParallax.ParallaxLayers.Count - 1)
                {
                    if (_proCamera2DParallax.ParallaxLayers[i].Speed > _proCamera2DParallax.ParallaxLayers[i + 1].Speed)
                        areSpeedsNotOrdered = true;
                }

                // Detect empty layer masks
                if (_proCamera2DParallax.ParallaxLayers[i].LayerMask.value == 0)
                    isLayerMaskEmpty = true;

                // Detect equal layer masks
                for (int j = 0; j < _proCamera2DParallax.ParallaxLayers.Count; j++)
                {
                    if (i != j && _proCamera2DParallax.ParallaxLayers[i].LayerMask.value == _proCamera2DParallax.ParallaxLayers[j].LayerMask.value)
                        isLayerMaskOverlapping = true;
                }

                // Remove layers with no camera assigned
                if (_proCamera2DParallax.ParallaxLayers[i].ParallaxCamera == null)
                {
                    _proCamera2DParallax.ParallaxLayers.RemoveAt(i);
                    EditorGUIUtility.ExitGUI();
                    continue;
                }
                
                // Apply layer masks as camera culling mask
                _proCamera2DParallax.ParallaxLayers[i].ParallaxCamera.cullingMask = _proCamera2DParallax.ParallaxLayers[i].LayerMask;

                // Setup clear flags
                if (_proCamera2DParallax.ParallaxLayers[0].Speed > 1)
                {
                    _proCamera2D.GameCamera.clearFlags = CameraClearFlags.SolidColor;
                    _proCamera2DParallax.ParallaxLayers[i].ParallaxCamera.clearFlags = CameraClearFlags.Depth;
                }
                else
                {
                    _proCamera2D.GameCamera.clearFlags = CameraClearFlags.Depth;

                    if (i > 0)
                        _proCamera2DParallax.ParallaxLayers[i].ParallaxCamera.clearFlags = CameraClearFlags.Depth;
                }
            }

            // Show warnings
            if (areSpeedsNotOrdered)
                EditorGUILayout.HelpBox("Parallax layer speeds are not ordered.", MessageType.Warning, true);

            if (isLayerMaskEmpty)
                EditorGUILayout.HelpBox("One or more layer mask is empty.", MessageType.Warning, true);

            if (isLayerMaskOverlapping)
                EditorGUILayout.HelpBox("Two or more cameras are rendering the same layers. Check your Layer Masks.", MessageType.Warning, true);


            // Setup depths
            for (int i = 0; i < _proCamera2DParallax.ParallaxLayers.Count; i++)
            {
                if (_proCamera2DParallax.ParallaxLayers[i].Speed > 1)
                    _proCamera2DParallax.ParallaxLayers[i].ParallaxCamera.depth = _proCamera2D.GameCamera.depth + (i + 1);
                else
                    _proCamera2DParallax.ParallaxLayers[i].ParallaxCamera.depth = _proCamera2D.GameCamera.depth - _proCamera2DParallax.ParallaxLayers.Count + (i);
            }

            // Show correct axis name
            var hAxis = "";
            var vAxis = "";
            switch (_proCamera2D.Axis)
            {
                case MovementAxis.XY:
                    hAxis = "X";
                    vAxis = "Y";
                    break;

                case MovementAxis.XZ:
                    hAxis = "X";
                    vAxis = "Z";
                    break;

                case MovementAxis.YZ:
                    hAxis = "Y";
                    vAxis = "Z";
                    break;
            }

            // Follow X
            _tooltip = new GUIContent("Parallax " + hAxis, "Should the parallax occur on the horizontal axis?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ParallaxHorizontal"), _tooltip);

            // Follow Y
            _tooltip = new GUIContent("Parallax " + vAxis, "Should the parallax occur on the vertical axis?");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ParallaxVertical"), _tooltip);

            // Root Position
            _tooltip = new GUIContent("Root Position", "The position at which your camera is going to start.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("RootPosition"), _tooltip);

            // Reset offset button
            if (GUILayout.Button("Set Root Position"))
            {
                switch (_proCamera2D.Axis)
                {
                    case MovementAxis.XY:
                        _proCamera2DParallax.RootPosition = new Vector3(_proCamera2D.transform.localPosition.x, _proCamera2D.transform.localPosition.y, 0);
                        break;

                    case MovementAxis.XZ:
                        _proCamera2DParallax.RootPosition = new Vector3(_proCamera2D.transform.localPosition.x, 0, _proCamera2D.transform.localPosition.z);
                        break;

                    case MovementAxis.YZ:
                        _proCamera2DParallax.RootPosition = new Vector3(0, _proCamera2D.transform.localPosition.y, _proCamera2D.transform.localPosition.z);
                        break;
                }
            }


            if (serializedObject.ApplyModifiedProperties())
            {
                for (int i = 0; i < _proCamera2DParallax.ParallaxLayers.Count; i++)
                {
                    _proCamera2DParallax.ParallaxLayers[i].ParallaxCamera.gameObject.name = "ParallaxCamera (" + _proCamera2DParallax.ParallaxLayers[i].Speed + ")";
                }
            }
        }

        void AddParallaxLayer()
        {
            var go = new GameObject("ParallaxCamera");
            var cam = go.AddComponent<Camera>();
            EditorUtility.CopySerialized(_proCamera2D.GameCamera, cam);
            go.transform.parent = _proCamera2D.transform;
            go.transform.localPosition = Vector3.zero;

            cam.cullingMask = 0;
            cam.clearFlags = CameraClearFlags.Depth;

            var parallaxLayer = new ProCamera2DParallaxLayer();
            parallaxLayer.ParallaxCamera = cam;
            parallaxLayer.Speed = 1;

            #if PC2D_TK2D_SUPPORT
            if (_proCamera2D.GetComponent<tk2dCamera>() != null)
            {
                var tk2dCam = go.AddComponent<tk2dCamera>();
                tk2dCam.InheritConfig = _proCamera2D.GetComponent<tk2dCamera>();
                parallaxLayer.ParallaxCameraTk2d = tk2dCam;
            }
            #endif

            _proCamera2DParallax.ParallaxLayers.Add(parallaxLayer);
        }
    }
}