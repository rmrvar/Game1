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
                if (PollForMove(out var direction) && RoomEntity.CanMove(direction.Value))
                {
                    yield return IE_HandleMove(direction.Value);
                    yield break;
                }
                if (PollForAttack())
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
                if (PollForMove(out var direction))
                {
                    RoomEntity.Face(direction.Value);
                    ShowAttackIndicators();
                }
                if (PollForAttack())  // Confirm the attack
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
                RoomEntity.Move(
                    direction, 
                    0.5F,
                    () => TreasureManager.Instance.TryToLoot(RoomEntity.Position)
                  );
                yield return new WaitUntil(() => !RoomEntity.IsMoving);
            }
        }
        
        private bool PollForAttack()
        {
            return Input.GetButtonDown("Jump");
        }

        private bool PollForMove(out EDirection? direction)
        {
            Vector2Int move;
            var horz = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            var vert = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
            if (horz != 0)
            {
                move = new Vector2Int(horz, 0);
            } else 
            if (vert != 0)
            {
                move = new Vector2Int(0, vert);
            }
            else
            {
                direction = null;
                return false;
            }
            direction = move.ToEDirection();
            return true;
        }
        
        private bool _isBusy;
    }
}
