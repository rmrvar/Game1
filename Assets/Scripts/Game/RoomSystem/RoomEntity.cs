using System.Threading;
using UnityEngine;
using Game.Common;
using Game.Misc;

namespace Game.RoomSystem
{
    public class RoomEntity : MonoBehaviour
    {
        [SerializeField] 
        private SpriteRenderer _spriteRenderer;
        
        [field: SerializeField]
        public bool OccupiesSpace { get; set; } = true;

        public event System.Action OnMoved;
        public event System.Action OnTurned;
        
        // Setting Position directly will cancel any ongoing movement.
        public Vector2Int Position
        {
            get => _position; 
            set
            {
                if (_moveCoroutine != null)
                {
                    StopCoroutine(_moveCoroutine);
                    _moveCoroutine = null;
                }
                _position = value;
                transform.position = new Vector3(value.x, value.y, transform.position.z);
            }
        }
        private Vector2Int _position;

        public bool IsMoving { get; private set; }
        
        public EDirection Direction { get; private set; } = EDirection.RIGHT;
        
        private void Awake()
        {
            Position = new Vector2Int(
                Mathf.FloorToInt(transform.position.x), 
                Mathf.FloorToInt(transform.position.y)
              );
        }

        private void Start()
        {
            if (_spriteRenderer != null)
            {
                Direction = _spriteRenderer.flipX
                    ? EDirection.LEFT
                    : EDirection.RIGHT;   
            }
        }

        private void OnEnable()
        {
            Room.Instance.AddRoomEntity(this);
        }

        private void OnDisable()
        {
            Room.Instance.RemRoomEntity(this);
        }

        public void Face(EDirection direction)
        {
            if (Direction == EDirection.LEFT  && direction == EDirection.RIGHT 
            ||  Direction == EDirection.RIGHT && direction == EDirection.LEFT)
            {
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
                }
            }
            Direction = direction;
            OnTurned?.Invoke();
        }

        public bool CanMove(EDirection direction)
        {
            var position = Position + direction.ToVector2Int();
            return Room.Instance.CanMoveInto(position);
        }
        
        public bool Move(
            EDirection direction, 
            float duration = 0.0F, 
            System.Action onCompleted = null,
            CancellationToken cancellationToken = default
          )
        {
            Face(direction);

            if (!CanMove(direction))
            {
                return false;
            }
            
            // Reserve the destination position.
            var position2 = Position + direction.ToVector2Int();
            _position = position2;
            
            // TODO: Replace with LeanTween or DOTween.
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
                // Snap position to the grid if movement interrupted (shouldn't happen).
                transform.position = new Vector3(Position.x, Position.y);  
            }

            IsMoving = true;
            _moveCoroutine = StartCoroutine(LerpAnimation.IE_MoveTo(
                transform,
                new Vector3(position2.x, position2.y, transform.position.z),
                duration,
                () =>
                {
                    IsMoving = false;
                    transform.position = new Vector3(_position.x, _position.y, transform.position.z);  // Fixes potential rounding errors.
                    _moveCoroutine = null;  // Call before onCompleted(). Otherwise, cannot cancel new coroutine if Move is called from there.
                    onCompleted?.Invoke();
                },
                cancellationToken
              ));
            OnMoved?.Invoke();
            return true;
        }

        private Coroutine _moveCoroutine;
    }
}