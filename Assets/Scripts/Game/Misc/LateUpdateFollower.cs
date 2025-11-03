using UnityEngine;

namespace Game.Misc
{
    public class LateUpdateFollower : MonoBehaviour
    {
        [SerializeField] 
        private Transform _followTarget;

        private void LateUpdate()
        {
            transform.position = new Vector3(
                _followTarget.position.x,
                _followTarget.position.y,
                transform.position.z
              );
        }
    }
}
