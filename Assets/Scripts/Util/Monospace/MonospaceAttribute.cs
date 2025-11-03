using UnityEngine;

namespace Util.Monospace
{
    public class MonospaceAttribute : PropertyAttribute
    {
        public readonly bool IsTextArea;
        public readonly int MinLines;
        public readonly int MaxLines;

        public MonospaceAttribute()
        {
            IsTextArea = false;
            MinLines = -1;
            MaxLines = -1;
        }
        
        public MonospaceAttribute(int minLines, int maxLines)
        {
            IsTextArea = true;
            MinLines = minLines;
            MaxLines = maxLines;
        }
    }
}