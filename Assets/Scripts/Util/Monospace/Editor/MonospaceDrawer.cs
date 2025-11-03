using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Util.Monospace.Editor
{
    [CustomPropertyDrawer(typeof(MonospaceAttribute))]
    public class MonospaceDrawer : PropertyDrawer
    {
        private static Font _monoFont;
        private Vector2 _scroll; // Scroll position for each property
        private string _text;

        private static Font GetMonoFont()
        {
            if (_monoFont == null)
            {
                _monoFont = Font.CreateDynamicFontFromOSFont(
                    new[] { "Consolas", "Courier New", "Menlo", "Monaco" },
                    12
                );
            }

            return _monoFont ?? EditorStyles.textField.font;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [Monospace] only on string fields.");
                return;
            }

            var attr = (MonospaceAttribute)attribute;

            if (attr.IsTextArea)
            {
                OnGUI_TextArea(position, property, label);
            }
            else
            {
                OnGUI_String(position, property, label);
            }
        }

        private void OnGUI_String(Rect position, SerializedProperty property, GUIContent label)
        {
            var style = new GUIStyle(EditorStyles.textField) { font = GetMonoFont() };

            EditorGUI.BeginChangeCheck();
            string newVal = EditorGUI.TextField(position, label, property.stringValue, style);
            if (EditorGUI.EndChangeCheck())
                property.stringValue = newVal;
        }

        private void OnGUI_TextArea(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (MonospaceAttribute)attribute;
            var font = GetMonoFont();

            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            // Prepare style
            var textAreaStyle = new GUIStyle(EditorStyles.textArea) { font = font };

            // Calculate content height
            var contentHeight = textAreaStyle.CalcHeight(new GUIContent(_text), position.width);
            var minHeight = textAreaStyle.CalcHeight(
                new GUIContent(string.Concat(Enumerable.Repeat("Line\n", attr.MinLines)).TrimEnd()),
                position.width
            );
            var maxHeight = textAreaStyle.CalcHeight(
                new GUIContent(string.Concat(Enumerable.Repeat("Line\n", attr.MaxLines)).TrimEnd()),
                position.width
            );
            
            EditorGUI.BeginChangeCheck();
            
            GUILayout.BeginScrollView(
                _scroll,
                false,
                contentHeight > maxHeight,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(Mathf.Clamp(contentHeight + 5, minHeight + 5, maxHeight + 5))
            );
            GUI.SetNextControlName("__UTIL_MONO_EDITOR_MONOSPACEDRAWER_TextArea");
            _text = GUILayout.TextArea(
                property.stringValue,
                textAreaStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(Mathf.Max(minHeight, contentHeight))
              );

            // var textAreaRect = new Rect(
            //     new Vector2(position.x, position.y + EditorGUIUtility.singleLineHeight),
            //     new Vector2(position.width, Mathf.Max(minHeight, contentHeight))
            // );
            
            GUILayout.EndScrollView();
            
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = _text;

                // if (GUI.GetNameOfFocusedControl() == "__UTIL_MONO_EDITOR_MONOSPACEDRAWER_TextArea")
                // {
                //     var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                //     var cursorIndex = editor.cursorIndex;
                //
                //     var cursorPixelPos = textAreaStyle.GetCursorPixelPosition(
                //         textAreaRect,
                //         new GUIContent(_text),
                //         cursorIndex
                //     );
                //     Debug.Log($"Scroll y : {_scroll.y} Cursor y : {cursorPixelPos.y} textAreaRect: {textAreaRect}");
                //     if (cursorPixelPos.y < _scroll.y)
                //     {
                //         _scroll.y = cursorPixelPos.y;
                //     } else 
                //     if (cursorPixelPos.y > _scroll.y + textAreaRect.height)
                //     {
                //         _scroll.y = cursorPixelPos.y - textAreaRect.height;
                //     }
                // }
            }

            EditorGUI.EndProperty();
        }
    }
}
