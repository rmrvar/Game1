#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Game.Ui
{
    public class EnemyHealthHUD : MonoBehaviour
    {
        [SerializeField] private Transform[] _hearts;

        [SerializeField] private int _pixelOffsetX = 0;
        [SerializeField] private int _pixelOffsetY = 10;
        [SerializeField] private int _pixelSpacing = 1;
        [SerializeField] private bool _isOddWidth = true;
        [SerializeField] private bool _isOddHeight = true;
        
        public void Show(int numHearts)
        {
            Debug.Assert(numHearts <= _hearts.Length, "Attempted to show more hearts than is possible!");
            
            MakePixelPerfect(numHearts);
            for (int i = 0; i < _hearts.Length; ++i)
            {
                _hearts[i].gameObject.SetActive(i < numHearts);
            }
        }

        private void OnValidate()
        {
            MakePixelPerfect(_hearts.Length);

#if UNITY_EDITOR
            // mark dirty so Unity serializes it
            EditorUtility.SetDirty(this);
#endif
        }

        private void MakePixelPerfect(int numHearts)
        {
            // Options:
            // odd width , odd amount  (5*3 + 2 = 17, not fine)
            // odd width , even amount (5*4 + 3 = 23, not fine)
            // even width, odd amount  (4*3 + 2 = 14, fine)
            // even width, even amount (4*4 + 3 = 19, not fine)
            var isOddWidth = _isOddWidth || (!_isOddWidth && numHearts % 2 == 0);
            var horizontalLayoutGroup = GetComponentInChildren<HorizontalLayoutGroup>();
            horizontalLayoutGroup.spacing = _pixelSpacing / 16.0F;
            horizontalLayoutGroup.transform.localPosition = new Vector3(
                (_pixelOffsetX + 8 + (isOddWidth ? 0.5F : 0)) / 16.0F,
                (_pixelOffsetY + 8 + (_isOddHeight ? 0.5F : 0)) / 16.0F,
                horizontalLayoutGroup.transform.localPosition.z
            );
        }
    }
}
