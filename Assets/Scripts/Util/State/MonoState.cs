using System.Collections.Generic;
using UnityEngine;

namespace Util.State
{
    [DisallowMultipleComponent]
    public abstract class MonoState : MonoBehaviour, IGraphNode
    {
        public bool IsActive
        {
            get => isActiveAndEnabled;
            private set
            {
                gameObject.SetActive(value);
                enabled = value;
            }
        }
        
        public bool IsInitialized { get; private set; }

        public abstract void OnInit(GraphInstance instance);

        public virtual void OnEnter(GraphInstance instance, Dictionary<string, object> args)
        {
            IsActive = true;
        }

        public virtual void OnExit(GraphInstance instance)
        {
            IsActive = false;
        }
    }
}
