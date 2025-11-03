using UnityEngine;

namespace Util.StateMachine
{
    [DisallowMultipleComponent]
    public abstract class MonoState : MonoBehaviour, IState
    {
        public bool IsActive
        {
            get => isActiveAndEnabled;
            set
            {
                gameObject.SetActive(value);
                enabled = value;
            }
        }

        public abstract void OnEnter();
        public abstract void OnExit();
    }
    
    public abstract class MonoState<T> : MonoState, IState<T>
    {
        [field: SerializeField]
        public T Key { get; private set; }
    }
}
