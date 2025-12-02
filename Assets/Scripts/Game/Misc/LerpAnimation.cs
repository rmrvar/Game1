using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Game.Misc
{
    public static class LerpAnimation
    {
        public static IEnumerator IE_MoveTo(
            Transform transform,
            Vector3 to,
            float duration = 0,
            Action<float> onCompleted = null,
            CancellationToken cancellationToken = default
          )
        {
            var fromTo = to - transform.position;
            
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
                
                elapsedTime += Time.deltaTime;
                transform.position += fromTo * (Time.deltaTime / duration);

                yield return null;
            }

            transform.position = to;  // Fixes potential rounding errors.
            onCompleted?.Invoke(elapsedTime - duration);
        }
    }
}
