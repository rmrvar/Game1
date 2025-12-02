using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.RoomSystem
{
    public class Room : Util.MonoSingleton<Room>
    {
        public Tilemap Tilemap { get; private set; }

        public Vector2Int GetFreePosition()
        {
            int count = _validPositions.Count;
            var index = Random.Range(0, count);
            return _validPositions[index];
        }
        
        public void SetTilemap(Tilemap tilemap)
        {
            Tilemap = tilemap;
            _validPositions = new List<Vector2Int>();
            _blockedPositions = new HashSet<Vector2Int>();
            
            var tileData = new TileData();
            var bounds = Tilemap.cellBounds;
            for (var dx = 0; dx < bounds.size.x; ++dx)
            for (var dy = 0; dy < bounds.size.y; ++dy)
            {
                var position = new Vector3Int(bounds.min.x + dx, bounds.min.y + dy, 0);
                var tile = Tilemap.GetTile(position);
                if (tile == null)
                {
                    continue;
                }

                var position2d = new Vector2Int(position.x, position.y);
                _validPositions.Add(position2d);
                tile.GetTileData(position, Tilemap, ref tileData);
                if (tileData.colliderType != Tile.ColliderType.None)
                {
                    _blockedPositions.Add(position2d);
                }
            }   
        }
        
        public void AddRoomEntity(RoomEntity roomEntity)
        {
            _roomEntities.Add(roomEntity);
        }

        public void RemRoomEntity(RoomEntity roomEntity)
        {
            _roomEntities.Remove(roomEntity);
        }
        
        public bool CanMoveInto(Vector2Int position)
        {
            if (IsCollisionAt(position))
            {
                return false;
            }
            var roomEntities = GetRoomEntitiesAt(position);
            foreach (var roomEntity in roomEntities)
            {
                if (roomEntity != null && roomEntity.OccupiesSpace)
                {
                    return false;
                }
            }
            return true;
        }

        public List<RoomEntity> GetRoomEntitiesAt(Vector2Int position)
        {
            return _roomEntities.Where(entity => entity.Position == position).ToList();
        }

        public bool IsCollisionAt(Vector2Int position)
        {
            return _blockedPositions.Contains(position);
        }

        private List<Vector2Int> _validPositions;
        private HashSet<Vector2Int> _blockedPositions;
        private readonly HashSet<RoomEntity> _roomEntities = new();
    }
}
