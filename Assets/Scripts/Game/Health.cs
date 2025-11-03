using UnityEngine;

namespace Game.Misc
{
    public class Health : MonoBehaviour
    {
        [field: SerializeField]
        public int MaxHearts { get; private set; } = 3;
        public int NumHearts { get; private set; }

        public event System.Action OnHurt;
        public event System.Action OnHealed;
        public event System.Action OnDied;
        
        public void Hurt(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Hurt amount cannot be negative!");
                return;
            }
            NumHearts -= amount;
            NumHearts = Mathf.Max(0, NumHearts);
            OnHurt?.Invoke();
            if (NumHearts == 0)
            {
                OnDied?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (amount < 0)
            {
                Debug.LogWarning("Heal amount cannot be negative!");
                return;
            }
            NumHearts += amount;
            NumHearts = Mathf.Min(MaxHearts, NumHearts);
            OnHealed?.Invoke();
        }

        private void Awake()
        {
            NumHearts = MaxHearts;
        }
    }
}
