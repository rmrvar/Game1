using System.Collections.Generic;
using System.Linq;
using Game.RoomSystem;
using Game.Ui;
using UnityEngine;

namespace Game.Treasure
{
    public class TreasureManager : Util.MonoSingleton<TreasureManager>
    {
        [SerializeField] 
        private RoomEntity[] _treasurePrefabs;

        [SerializeField]
        private TreasureUi _treasureUi;
        
        public RoomEntity SpawnTreasure()
        {
            if (_treasurePrefabs.Length == 0)
            {
                Debug.LogError("Cannot spawn treasure! No treasure prefab assigned!");
                return null;
            }
            
            var treasurePrefab = _treasurePrefabs[Random.Range(0, _treasurePrefabs.Length)]
                .GetComponent<RoomEntity>();

            var roomEntity = RoomEntityManager.Instance.SpawnRoomEntity(
                treasurePrefab,
                3,
                false
              );
            _treasures.Add(roomEntity);
            return roomEntity;
        }

        public RoomEntity SpawnTreasure(Vector2Int position)
        {            
            if (_treasurePrefabs.Length == 0)
            {
                Debug.LogError("Cannot spawn treasure! No treasure prefab assigned!");
                return null;
            }
            
            var treasurePrefab = _treasurePrefabs[Random.Range(0, _treasurePrefabs.Length)]
                .GetComponent<RoomEntity>();
            
            var roomEntity = RoomEntityManager.Instance.SpawnRoomEntity(
                treasurePrefab,
                position
              );
            _treasures.Add(roomEntity);
            return roomEntity;
        }
        
        //
        // public void DespawnTreasure(RoomEntity treasure)
        // {
        //     _treasures.Remove(treasure);
        //     Destroy(treasure.gameObject);
        // }
        //
        // public RoomEntity GetTreasure(Vector2Int position)
        // {
        //     return _treasures.FirstOrDefault(roomEntity => roomEntity.Position == position);
        // }
        public void TryToLoot(Vector2Int position)
        {
            var treasure = _treasures.FirstOrDefault(roomEntity => roomEntity.Position == position);
            if (treasure == null)
            {
                return;
            }
            Destroy(treasure.gameObject);
            _treasureUi.SetTreasureCount(++_treasureCount);
        }

        public void Reset()
        {
            ClearTreasures();
            _treasureCount = 0;
            _treasureUi.SetTreasureCount(0);
        }

        public void ClearTreasures()
        {
            foreach (var treasure in _treasures)
            {
                if (treasure != null)
                {
                    Destroy(treasure.gameObject);
                }
            }
            _treasures.Clear();
        }
        
        private List<RoomEntity> _treasures = new();
        private int _treasureCount = 0;
    }
}