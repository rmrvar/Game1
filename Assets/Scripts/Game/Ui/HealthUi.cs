using System.Collections.Generic;
using Game.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Ui
{
    public class HealthUi : MonoBehaviour
    {
        [SerializeField] 
        private Slider _heartPrefab;

        public Health Health
        {
            get => _health;
            set
            {
                CleanUpOld();
                _health = value;
                SetUpNew();
                Refresh();
            }
        }

        public void Refresh()
        {            
            for (int i = 0; i < Health.MaxHearts; ++i)
            {
                _hearts[i].value = (i < Health.NumHearts) ? 1 : 0;
            }
        }
        
        private void CleanUpOld()
        {
            if (_health == null)
            {
                return;
            }
            _health.OnHurt -= OnHealthChanged;
            _health.OnHealed -= OnHealthChanged;
            foreach (var heart in _hearts)
            {
                Destroy(heart.gameObject);
            }
            _hearts.Clear();
        }

        private void SetUpNew()
        {
            _health.OnHurt += OnHealthChanged;
            _health.OnHealed += OnHealthChanged;
            
            for (int i = 0; i < Health.MaxHearts; ++i)
            {
                var heart = Instantiate(_heartPrefab, transform);
                heart.value = (i < Health.NumHearts) ? 1 : 0;
                _hearts.Add(heart);
            }
        }
        
        private void OnHealthChanged()
        {
            Refresh();
        }

        private Health _health;
        private List<Slider> _hearts = new();
    }
}
