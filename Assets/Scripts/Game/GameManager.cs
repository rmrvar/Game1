using System.Collections.Generic;
using Game.Common;
using UnityEngine;
using Util;

namespace Game
{
    using GameState;
    
    public class GameManager : MonoSingleton<GameManager>
    {
        public void Start()
        {
            Application.targetFrameRate = 30;

            GameStateManager.Instance.StateMachine.SetState(
                Constants.GameState.MENU_STATE_KEY,
                new Dictionary<string, object>()
                {
                    { Constants.MenuState.SUBSTATE_ARG_NAME, Constants.MenuState.START_SUBSTATE_KEY }
                }
              );
        }
    }
}
