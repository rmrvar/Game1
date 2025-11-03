using UnityEngine;

namespace Util.StateMachine
{
    public abstract class MonoSingletonState<T1, T2> : MonoState<T1> where T2 : MonoSingletonState<T1, T2>
    {
        public static T2 Instance { get; private set; }

        protected virtual void Awake()
        {
            Debug.Log($"Waking up state {gameObject.name}!");
            Debug.Assert(Instance == null, $"{GetType().Name} already exists!");
            Instance = (T2) this;
        }
    }
}
