using _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Behaviours;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace _Project.Scripts.Gameplay.Features.VisualFeature.UIFeature.ButtonFeature.Editor
{
    [CustomEditor(typeof(DragonButton))]
    public class DragonButtonEditor : ButtonEditor
    {
        private SerializedProperty _entityTemplateProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _entityTemplateProperty = serializedObject.FindProperty("EntityCfg");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(_entityTemplateProperty);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed) 
                EditorUtility.SetDirty(target);
            
            base.OnInspectorGUI();
        }
    }
}