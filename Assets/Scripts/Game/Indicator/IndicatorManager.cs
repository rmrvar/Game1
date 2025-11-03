using System.Collections.Generic;
using Game.Common;
using UnityEngine;
using Util;

namespace Game.Indicator
{
    public class IndicatorManager : MonoSingleton<IndicatorManager>
    {
        public IEnumerable<Transform> RequestIndicators(
            Transform indicatorPrefab,
            IndicatorPattern pattern, 
            Vector2Int position, 
            EDirection direction
          )
        {
            var indicators = new List<Transform>();
            foreach (var pos in pattern.GetPositions(position, direction))
            {
                var indicator = Instantiate(
                    indicatorPrefab, 
                    new Vector3(pos.x, pos.y, -1),
                    Quaternion.identity
                  );
                indicators.Add(indicator);
            }
            return indicators;
        }

        public void ReleaseIndicators(IEnumerable<Transform> indicators)
        {
            foreach (var indicator in indicators)
            {
                Destroy(indicator.gameObject);
            }
        }
    }
}
