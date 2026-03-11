using System.Collections;
using System.Threading;
using Game.Animation;
using Game.Common;
using Game.RoomSystem;
using Game.Treasure;
using UnityEngine;

namespace Game.Character
{
    public class EnemyCharacter : Character
    {
        [SerializeField] 
        private int _attackWindupTime = 1;
        
        protected override void Kill()
        {
            CharacterManager.Instance.KillEnemy(this);
            if (Random.value > 0.5F)
            {
                TreasureManager.Instance.SpawnTreasure(RoomEntity.Position);   
            }
            base.Kill();
        }
        
        public override IEnumerator IE_ExecuteTurn(CancellationToken token)
        {
            CharacterManager.Instance.OnEnemyStartedAction?.Invoke(this);

            if (_isWindingUp)
            {
                ++_windupCounter;
                if (_windupCounter < _attackWindupTime)
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
                                var player = roomEntity.GetComponent<PlayerCharacter>();
                                if (player != null)
                                {
                                    // TODO: Problem killing player mid-turn.
                                    player.Health.Hurt(1);
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
            var playerPos = CharacterManager.Instance.Player.RoomEntity.Position;
            var fromTo = playerPos - RoomEntity.Position;
            if (Mathf.Abs(fromTo.x) + Mathf.Abs(fromTo.y) <= 1)
            {
                RoomEntity.Face(fromTo.ToEDirection());
                yield return IE_Attack();
                CharacterManager.Instance.OnEnemyEndedAction?.Invoke(this);
            }
            else
            {
                yield return IE_Pathfind();
            }
        }
        
        private IEnumerator IE_Pathfind()
        {
            var direction = Dijkstra.GetDirection(RoomEntity.Position);
            if (direction == null || !RoomEntity.CanMove(direction.Value))
            {
                CharacterManager.Instance.OnEnemyEndedAction?.Invoke(this);
                yield break;  // Enemy is stuck.
            }
            AnimationManager.Instance.PlayMoveAnimation(
                RoomEntity,
                direction.Value,
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
    }
}
