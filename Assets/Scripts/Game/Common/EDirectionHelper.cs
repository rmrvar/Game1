using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Common
{
    public static class EDirectionHelper
    {
        public static EDirection GetRandomDirection()
        {
            var rand = Random.Range(0, 4);
            return (EDirection) rand;
        }
        
        public static Vector2Int ToVector2Int(this EDirection direction)
        {
            var x = direction switch
            {
                EDirection.LEFT => -1,
                EDirection.RIGHT => +1,
                _ => 0
            };
            var y = direction switch
            {
                EDirection.DOWN => -1,
                EDirection.UP => +1,
                _ => 0,
            };
            return new Vector2Int(x, y);
        }

        public static EDirection Opposite(this EDirection direction)
        {
            return direction switch
            {
                EDirection.LEFT => EDirection.RIGHT,
                EDirection.RIGHT => EDirection.LEFT,
                EDirection.DOWN => EDirection.UP,
                EDirection.UP => EDirection.DOWN,
                _ => throw new Exception($"ToEDirection(): Invalid direction {direction}!")
            };
        }
        
        public static Vector2 ToVector2(this EDirection direction)
        {
            return ToVector2Int(direction);
        }

        public static Vector3 ToVector3(this EDirection direction)
        {
            return ToVector2(direction);
        }

        public static EDirection ToEDirection(this Vector2Int direction)
        {
            if (direction.x == -1 && direction.y == 0)
            {
                return EDirection.LEFT;
            }
            if (direction.x == +1 && direction.y == 0)
            {
                return EDirection.RIGHT;
            }
            if (direction.x == 0 && direction.y == -1)
            {
                return EDirection.DOWN;
            }
            if (direction.x == 0 && direction.y == +1)
            {
                return EDirection.UP;
            }
            throw new Exception($"ToEDirection(): Invalid direction {direction}!");
        }
    }
}
