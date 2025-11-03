using UnityEngine;

namespace Game.RoomSystem
{
    public class Gate : MonoBehaviour
    {
        [ContextMenu("Raise")]
        public void Raise()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) Awake();
#endif
            SetRaisedness(true);
        }

        [ContextMenu("Lower")]
        public void Lower()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) Awake();
#endif
            SetRaisedness(false);
        }

        private void SetRaisedness(bool isRaised)
        {
            foreach (var roomEntity in _roomEntities)
            {
                roomEntity.gameObject.SetActive(isRaised);
            }
        }
        
        private void Awake()
        {
            _roomEntities = GetComponentsInChildren<RoomEntity>(includeInactive: true);
        }

        private RoomEntity[] _roomEntities;
    }
}
