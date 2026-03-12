using System;
using System.Collections;
using System.Threading;
using Game.Animation;
using Game.Common;
using Game.RoomSystem;
using UnityEngine;

namespace Game.Character
{
    public class TutorialEnemyCharacter : EnemyCharacter
    {
        [SerializeField]
        private bool _isDamsel;

        protected override void OnEnable()
        {
            base.OnEnable();
            CharacterManager.Instance.OnEnemyKilled += (_) => _isDamselDead = true;
        }
        
        public override IEnumerator IE_ExecuteTurn(CancellationToken token)
        {
            if (!_isDamsel && _isDamselDead)
            {
                yield return base.IE_ExecuteTurn(token);
                yield break;
            }
            
            CharacterManager.Instance.OnEnemyStartedAction?.Invoke(this);
            
            if (_isDamsel)
            {
                yield return IE_Pathfind();
            }
            else
            {
                if (_isWindingUp)
                {
                    ++_windupCounter;
                    if (_windupCounter < 1)
                    {
                        yield break;   
                    }
    
                    foreach (var attackIndicator in AttackIndicators)
                    {
                        var position = new Vector2Int((int)attackIndicator.position.x, (int)attackIndicator.position.y);
                        AnimationManager.Instance.PlayAttackAnimation(
                            AttackAnimationPrefab,
                            position,
                            () =>
                            {
                                var roomEntities = Room.Instance.GetRoomEntitiesAt(position);
                                foreach (var roomEntity in roomEntities)
                                {
                                    var health = roomEntity.GetComponent<Health>();
                                    if (health != null)
                                    {
                                        health.Hurt(1);
                                    }
                                }
                                CharacterManager.Instance.OnEnemyEndedAction?.Invoke(this);
                            }
                          );
                    }
                    HideAttackIndicators();
                    _isWindingUp = false;
                    _windupCounter = 0;
                    yield break;
                }
                RoomEntity.Face(EDirection.DOWN);
                yield return IE_Attack();
                CharacterManager.Instance.OnEnemyEndedAction?.Invoke(this);
            }
        }

                
        private IEnumerator IE_Pathfind()
        {
            var direction = EDirection.DOWN;
            if (direction == null || !RoomEntity.CanMove(direction))
            {
                CharacterManager.Instance.OnEnemyEndedAction?.Invoke(this);
                yield break;  // Enemy is stuck.
            }
            AnimationManager.Instance.PlayMoveAnimation(
                RoomEntity,
                direction,
                0.5F,
                onCompleted: (_) => CharacterManager.Instance.OnEnemyEndedAction?.Invoke(this)
            );
        }

        private IEnumerator IE_Attack()
        {
            ShowAttackIndicators();
            _isWindingUp = true;
            yield break;
        }

        private bool _isWindingUp;
        private int _windupCounter;
        private bool _isDamselDead;
    }
}