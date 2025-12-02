using Game.Common;
using UnityEngine;
using Util;

namespace Game
{
    public class PlayerInput : MonoSingleton<PlayerInput>
    {
        public void SetDeltaTimeForMove(float unusedDeltaTime)
        {
            _unusedDeltaTime = unusedDeltaTime;
            _timeOfLastMove = Time.time;
        }

        public float GetDeltaTimeForMove()
        {
            var deltaTime = _unusedDeltaTime + (Time.time - _timeOfLastMove);
            return deltaTime;
        }

        public bool PollForAttack()
        {
            return Input.GetButtonDown("Jump");
        }

        public bool PollForMove(out EDirection? direction)
        {
            Vector2Int move;
            var horz = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            var vert = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
            if (horz != 0)
            {
                move = new Vector2Int(horz, 0);
                direction = move.ToEDirection();
            } else 
            if (vert != 0)
            {
                move = new Vector2Int(0, vert);
                direction = move.ToEDirection();
            }
            else
            {
                direction = null;
            }
            
            if (_prevDirection != direction)
            {
                SetDeltaTimeForMove(0);
            }
            _prevDirection = direction;
            
            return direction != null;
        }
        
        private EDirection? _prevDirection;
        private float _unusedDeltaTime;
        private float _timeOfLastMove;
    }
}
