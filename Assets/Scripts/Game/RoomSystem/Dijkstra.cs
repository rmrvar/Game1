using System.Collections.Generic;
using Game.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.RoomSystem
{
    public static class Dijkstra
    {
        public static EDirection? GetDirection(Vector2Int position)
        {
            _map.TryGetValue(position, out var direction);
            return direction;
        }

        public static void SetDestination(Vector2Int position)
        {
            _map.Clear();
            
            var openQueue = new Queue<(Vector2Int, EDirection?)>();
            openQueue.Enqueue((position, null));

            while (openQueue.Count > 0)
            {
                var (pos1, dir) = openQueue.Dequeue();
                if (_map.ContainsKey(pos1))
                {
                    continue;  // Already processed
                }                    
                if (Vector2.Distance(pos1, position) > 10)
                {
                    continue;  // Too far
                }
                _map.Add(pos1, dir);
                if (!Room.Instance.CanMoveInto(pos1) && pos1 != position)
                {
                    continue;  // Can't continue.
                }
                var directions = new[] { EDirection.LEFT, EDirection.RIGHT, EDirection.DOWN, EDirection.UP };
                var indices = new List<int> { 0, 1, 2, 3 };
                while (indices.Count > 0)
                {
                    var i1 = Random.Range(0, indices.Count);
                    var i2 = indices[i1];
                    indices.RemoveAt(i1);
                    var direction = directions[i2];
                    var pos2 = pos1 + direction.ToVector2Int();
                    openQueue.Enqueue((pos2, direction.Opposite()));   
                }   
            }
        }
        
        private static readonly Dictionary<Vector2Int, EDirection?> _map = new();
    }
}
