using TMPro;
using UnityEngine;

namespace Game.Ui
{
    public class TreasureUi : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI _text;

        public void SetTreasureCount(int treasure)
        {
            _text.text = treasure.ToString();
        }
    }
}