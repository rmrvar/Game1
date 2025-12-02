using System.Collections;
using System.Threading;
using Game.Common;
using Game.GameState;
using Game.Misc;
using Game.RoomSystem;
using Game.Treasure;
using UnityEngine;

namespace Game.Character
{
    public class PlayerCharacter : Character
    {
        [SerializeField]
        private int _numPhases = 1;
        
        protected override void Kill()
        {
            CharacterManager.Instance.KillPlayer();
            base.Kill();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CharacterManager.Instance.OnEnemyEndedAction += OnEnemyEndedAction;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CharacterManager.Instance.OnEnemyEndedAction -= OnEnemyEndedAction;
        }

        private void OnEnemyEndedAction(EnemyCharacter enemy)
        {
            PlayerInput.Instance.SetDeltaTimeForMove(0);
        }
        
        public override IEnumerator IE_ExecuteTurn(CancellationToken token)
        {
            if (_isBusy)
            {
                Debug.LogError("Called PlayerCharacter.IE_ExecuteTurn while already in progress!");
                yield break;
            }
            _isBusy = true;
            
            for (int i = 0; i < _numPhases; ++i)
            {
                yield return IE_ExecutePhase();
                if (token.IsCancellationRequested)
                {  // Might've restarted TurnManager. Stop processing turn.
                    _isBusy = false;
                    yield break;
                }   
            }
            Dijkstra.SetDestination(RoomEntity.Position);
            _isBusy = false;
        }

        private IEnumerator IE_ExecutePhase()
        {
            while (true)
            {
                if (PlayerInput.Instance.PollForMove(out var direction) && RoomEntity.CanMove(direction.Value))
                {
                    yield return IE_HandleMove(direction.Value);
                    yield break;
                }

                // Player chose to not do anything, so reset deltas.
                PlayerInput.Instance.SetDeltaTimeForMove(0);
                
                if (PlayerInput.Instance.PollForAttack())
                {
                    bool isCancelled = true; 
                    yield return IE_HandleAttack(() => isCancelled = false);
                    if (!isCancelled)
                    {
                        yield return null;  // Wait a frame (cancels attack phase).
                        yield break;   
                    }
                }
                yield return null;  // Wait for input.
            }
        }
        
        private IEnumerator IE_HandleAttack(System.Action onComplete)
        {
            yield return null;  // Wait a frame

            ShowAttackIndicators();
            
            while (true)
            {
                if (PlayerInput.Instance.PollForMove(out var direction))
                {
                    RoomEntity.Face(direction.Value);
                    ShowAttackIndicators();
                }
                if (PlayerInput.Instance.PollForAttack())  // Confirm the attack
                {
                    foreach (var attackIndicator in AttackIndicators)
                    {
                        var roomEntity = Room.Instance.GetRoomEntityAt(
                            new Vector2Int((int) attackIndicator.position.x, (int) attackIndicator.position.y)
                          );
                        if (roomEntity != null)
                        {
                            var health = roomEntity.GetComponent<Health>();
                            if (health != null)
                            {
                                health.Hurt(1);
                            }
                        }
                    }
                    onComplete?.Invoke();
                    HideAttackIndicators();
                    yield break;
                }
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    HideAttackIndicators();
                    yield break;
                }
                yield return null;
            }
        }

        private IEnumerator IE_HandleMove(EDirection direction)
        {
            if (direction == EDirection.UP && RoomEntity.Position.y >= RoomGameState.Instance.RoomTransitionThresholdY)
            {
                yield return RoomGameState.Instance.IE_DoRoomTransition();
            }
            else
            {
                var unusedDeltaTime = PlayerInput.Instance.GetDeltaTimeForMove();
                RoomEntity.transform.position += direction.ToVector3() * (unusedDeltaTime / 1.0F);
                RoomEntity.Move(
                    direction, 
                    0.5F - unusedDeltaTime,
                    unusedDeltaTime =>
                    {
                        PlayerInput.Instance.SetDeltaTimeForMove(unusedDeltaTime);
                        TreasureManager.Instance.TryToLoot(RoomEntity.Position);
                    }
                  );
                yield return new WaitUntil(() => !RoomEntity.IsMoving);
            }
        }
        
        private bool _isBusy;
    }
}
