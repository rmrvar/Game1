using System.Collections.Generic;
using System.Linq;
using Game.Common;
using UnityEngine;
using Util.Monospace;

namespace Game.Indicator
{
    [CreateAssetMenu(
        fileName = "IndicatorPattern",
        menuName = "Game/Indicator Patterns",
        order = 0
      )]
    public class IndicatorPattern : ScriptableObject
    {
        [SerializeField, Monospace(3, 10), Tooltip("' ' = empty, 'x' = indicator, 'o' = origin.")] 
        private string _pattern =
            "xxxxx\n" +
            " xxx \n" +
            "  x  \n" +
            "  o  ";

        public IEnumerable<Vector2Int> Positions => _positions;
        [SerializeField, HideInInspector]
        private List<Vector2Int> _positions;

        public IEnumerable<Vector2Int> GetPositions(Vector2Int originPosition, EDirection direction)
        {
            return Positions.Select(pos =>
            {
                var up = direction.ToVector2Int();
                var right = new Vector2Int(up.y, -up.x);
                return originPosition + up * pos.y + right * pos.x;
            });
        }
        
        public bool Validate(out string error)
        {
            if (!_pattern.All(c => c is '\n' or ' ' or 'x' or 'o'))
            {
                error = "The pattern contains invalid symbols! ' ' = empty, 'x' = indicator, 'o' = origin";
                return false;
            }
            if (_pattern.Count(c => c == 'o') != 1)
            {
                error = "The pattern must have one and only one origin!";
                return false;
            }

            error = "";
            return true;
        }
        
        private void OnValidate()
        {
            Parse();
        }
        
        private void Parse()
        {
            _positions = new List<Vector2Int>();
            
            if (!Validate(out _))
            {
                return;
            }
            
            var lines = _pattern.Split('\n');
            
            int x = 0;
            int y = 0;
            
            // Find the origin.
            for (y = 0; y < lines.Length; ++y)
            {
                var line = lines[y];
                for (x = 0; x < line.Length; ++x)
                {
                    if (line[x] == 'o')
                    {
                        break;
                    }
                }
                if (x != line.Length)
                {
                    break;
                }
            }
            var origin = new Vector2Int(x, y);

            // Find the offsets.
            for (y = 0; y < lines.Length; ++y)
            {
                var line = lines[y];
                for (x = 0; x < line.Length; ++x)
                {
                    if (line[x] == 'x')
                    {
                        // y is flipped on purpose.
                        var offset = new Vector2Int(x - origin.x, origin.y - y);
                        _positions.Add(offset);
                    }
                }
            }
        }
    }
}
