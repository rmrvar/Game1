using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Util.StateMachine
{
    public class StateMachine<T>
    {
        public IState<T> CurrentState { get; private set; }

        public void AddState(IState<T> state)
        {
            _states.Add(state);
            state.IsActive = false;
        }
        
        public void SetState(IState<T> state)
        {
            if (CurrentState == state)
            {
                return;
            }
            
            if (CurrentState != null)
            {
                CurrentState.OnExit();
                state.IsActive = false;
            }

            CurrentState = state;
            
            if (CurrentState != null)
            {
                state.IsActive = true;
                CurrentState.OnEnter();
            }
        }

        public void SetState(T key)
        {
            var state = _states.FirstOrDefault(state => state.Key.Equals(key));
            Debug.Assert(state != null, $"State {key} not found!");
            SetState(state);
        }

        private readonly List<IState<T>> _states = new List<IState<T>>();
    }
}
