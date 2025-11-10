using UnityEngine;

namespace Util.State
{
    public abstract class MonoSingletonState<TThis> : MonoState
        where TThis : MonoSingletonState<TThis>
    {
        public static TThis Instance { get; private set; }

        protected virtual void Awake()
        {
            Debug.Log($"Waking up state {gameObject.name}!");
            Debug.Assert(Instance == null, $"{GetType().Name} already exists!");
            Instance = (TThis) this;
        }
    }
}
