using UnityEditor;
using UnityEngine;

namespace Game.Indicator.Editor
{
    [CustomEditor(typeof(IndicatorPattern))]

    public class IndicatorPatternEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var pattern = (IndicatorPattern) target;
            
            GUIStyle style = new GUIStyle(EditorStyles.label);

            if (pattern.Validate(out var validationMessage))
            {
                validationMessage = "The pattern is valid!";
                style.normal.textColor = Color.green;
            }
            else
            {
                style.normal.textColor = Color.red;
            }
            GUILayout.Label(validationMessage, style);
        }
    }
}
