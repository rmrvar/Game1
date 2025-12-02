using System.Linq;
using Game.RoomSystem;
using UnityEngine;
using Util;

namespace Game.Treasure
{
    public class RoomEntityManager : MonoSingleton<RoomEntityManager>
    {
        public RoomEntity SpawnRoomEntity(RoomEntity roomEntityPrefab, Vector2Int position)
        {
            var roomEntity = Instantiate(
                roomEntityPrefab,
                new Vector3(position.x, position.y),
                Quaternion.identity
            );
            return roomEntity;
        }
        
        // TODO: Make nicer.
        public RoomEntity SpawnRoomEntity(RoomEntity roomEntityPrefab, float radius = 3, bool ignoreOccupiesSpace = true)
        {   
            var positionBot = Vector2Int.zero;
            var positionTop = Vector2Int.zero;
            while (true)
            {
                var p = Random.insideUnitCircle * radius;
                positionBot.x = Mathf.RoundToInt(p.x);
                positionBot.y = Mathf.RoundToInt(p.y);
                positionTop = new Vector2Int(positionBot.x, positionBot.y + 12);
                // Since Room only knows about the bottom colliders, check there. But the room entities are spawning in the 
                // top, so check that there.
                if (Room.Instance.IsCollisionAt(positionBot))
                {
                    continue;
                }

                if (ignoreOccupiesSpace)
                {
                    var shouldSkip = Room.Instance.GetRoomEntitiesAt(positionTop).Any(roomEntity => 
                        roomEntity != null && roomEntity.OccupiesSpace
                    );
                    if (shouldSkip)
                    {
                        continue;
                    }
                }
                break;
            }

            return SpawnRoomEntity(roomEntityPrefab, positionTop);
        }    
    }
}