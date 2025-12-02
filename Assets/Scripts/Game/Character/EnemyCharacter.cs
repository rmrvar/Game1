using System.Collections;
using System.Threading;
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
                    var roomEntity = Room.Instance.GetRoomEntityAt(
                        new Vector2Int((int) attackIndicator.position.x, (int) attackIndicator.position.y)
                    );
                    if (roomEntity != null)
                    {
                        var player = roomEntity.GetComponent<PlayerCharacter>();
                        if (player != null)
                        {
                            player.Health.Hurt(1);
                        }
                    }
                }
                HideAttackIndicators();
                _isWindingUp = false;
                _windupCounter = 0;
                CharacterManager.Instance.OnEnemyEndedAction?.Invoke(this);
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
            if (direction == null)
            {
                CharacterManager.Instance.OnEnemyEndedAction?.Invoke(this);
                yield break;  // Enemy is stuck.
            }
            if (!RoomEntity.Move(
                direction.Value,
                0.5F,
                onCompleted: (_) => CharacterManager.Instance.OnEnemyEndedAction?.Invoke(this)
              ))
            {
                CharacterManager.Instance.OnEnemyEndedAction?.Invoke(this);
            }
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
