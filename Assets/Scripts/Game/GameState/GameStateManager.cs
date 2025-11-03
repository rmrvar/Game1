using UnityEngine;
using Util;
using Util.StateMachine;

namespace Game.GameState
{
    public class GameStateManager : MonoSingleton<GameStateManager>
    {
        [SerializeField] private MenuGameState _menuGameState;
        [SerializeField] private RoomGameState _roomGameState;

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new StateMachine<EGameState>();
            _stateMachine.AddState(_menuGameState);
            _stateMachine.AddState(_roomGameState);
        }

        public void SetGameState(EGameState state)
        {
            _stateMachine.SetState(state);
        }

        private StateMachine<EGameState> _stateMachine;
    }
}
