using UnityEngine;

namespace Util
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            Debug.Assert(Instance == null, $"{GetType().Name} already exists!");
            Instance = (T)this;
        }
    }
}
