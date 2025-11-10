using System.Collections.Generic;
using UnityEngine;
using Util;
using static Game.Common.Constants.GameState;

namespace Game.GameState
{
    public class GameStateManager : MonoSingleton<GameStateManager>
    {
        [SerializeField] private MenuGameState _menuGameState;
        [SerializeField] private RoomGameState _roomGameState;
        
        public Util.State.GraphInstance StateMachine { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            var stateGraph = Util.State.Graph.Builder
                .Start()
                .AddNode(MENU_STATE_KEY, _menuGameState)
                .AddNode(ROOM_STATE_KEY, _roomGameState)
                .AddEdge(MENU_STATE_KEY, ROOM_STATE_KEY)
                .AddEdge(ROOM_STATE_KEY, MENU_STATE_KEY)
                .Finish();
            StateMachine = new Util.State.GraphInstance(stateGraph);
        }
    }
}
