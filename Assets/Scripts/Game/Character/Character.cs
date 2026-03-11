using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Game.Indicator;
using Game.Misc;
using Game.RoomSystem;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField]
        protected Transform AttackAnimationPrefab;

        [SerializeField]
        private Sprite _spriteIdle;
        [SerializeField]
        private Sprite _spriteAttack;
        
        public abstract IEnumerator IE_ExecuteTurn(CancellationToken token);
        
        protected virtual void Kill()
        {
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
            GetComponentInChildren<SpriteRenderer>().sprite = _spriteAttack;
        }

        protected void HideAttackIndicators()
        {
            if (AttackIndicators != null)
            {
                IndicatorManager.Instance.ReleaseIndicators(AttackIndicators);
                AttackIndicators = null;
            }
            GetComponentInChildren<SpriteRenderer>().sprite = _spriteIdle;
        }

        protected virtual void OnEnable()
        {
            Health.OnDied += Kill;
        }

        protected virtual void OnDisable()
        {
            Health.OnDied -= Kill;
            HideAttackIndicators();
        }
        
        protected IEnumerable<Transform> AttackIndicators { get; private set; }
    }
}
