using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Game.Indicator;
using Game.Misc;
using Game.RoomSystem;
using UnityEngine;
using Util.TurnSystem;

namespace Game.Character
{
    public abstract class Character : MonoBehaviour
    {        
        [field: SerializeField]
        public RoomEntity RoomEntity { get; private set; }
        [field: SerializeField]
        public Health Health { get; private set; }
        public TurnManager<Character> TurnManager { get; set; }
        
        [SerializeField] 
        private Transform _attackIndicatorPrefab;
        [SerializeField]
        private IndicatorPattern _attackIndicatorPattern;
        
        public abstract IEnumerator IE_ExecuteTurn(CancellationToken token);
        
        protected virtual void Kill()
        {
            HideAttackIndicators();
            TurnManager.RemToken(this);
            Destroy(gameObject);
        }
        
        protected void ShowAttackIndicators()
        {
            if (AttackIndicators != null)
            {
                IndicatorManager.Instance.ReleaseIndicators(AttackIndicators);
                AttackIndicators = null;
            }
            AttackIndicators = IndicatorManager.Instance.RequestIndicators(
                _attackIndicatorPrefab, 
                _attackIndicatorPattern,
                RoomEntity.Position, 
                RoomEntity.Direction
            );
        }

        protected void HideAttackIndicators()
        {
            if (AttackIndicators != null)
            {
                IndicatorManager.Instance.ReleaseIndicators(AttackIndicators);
                AttackIndicators = null;
            }
        }

        private void OnEnable()
        {
            Health.OnDied += Kill;
        }

        private void OnDisable()
        {
            Health.OnDied -= Kill;
        }
        
        protected IEnumerable<Transform> AttackIndicators { get; private set; }
    }
}
