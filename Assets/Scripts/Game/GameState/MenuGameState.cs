using UnityEngine;
using Util.StateMachine;

namespace Game.GameState
{
    public class MenuGameState : MonoSingletonState<EGameState, MenuGameState>
    {
        public override void OnEnter()
        {
            Debug.Log("MenuGameState OnEnter");
        }

        public override void OnExit()
        {
            Debug.Log("MenuGameState OnExit");
        }
    }
}
