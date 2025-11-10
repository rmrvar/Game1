using System.Collections.Generic;
using Game.Common;
using UnityEngine;
using Util.State;

namespace Game.GameState
{
    public class MenuGameState : MonoSingletonState<MenuGameState>
    {
        [SerializeField]
        private Transform _root;
        [SerializeField]
        private Transform _startRoot;
        [SerializeField]
        private Transform _gameOverRoot;
        
        public override void OnInit(GraphInstance instance)
        {
            Debug.Log("MenuGameState OnInit");
        }
        
        public override void OnEnter(GraphInstance instance, Dictionary<string, object> args)
        {
            base.OnEnter(instance, args);
            Debug.Log("MenuGameState OnEnter");

            var substate = (string) args[Constants.MenuState.SUBSTATE_ARG_NAME];
            if (substate == Constants.MenuState.START_SUBSTATE_KEY)
            {
                _startRoot.gameObject.SetActive(true);
            }
            else
            {
                _gameOverRoot.gameObject.SetActive(true);
            }
            _root.gameObject.SetActive(true);
        }

        public override void OnExit(GraphInstance instance)
        {
            base.OnExit(instance);
            Debug.Log("MenuGameState OnExit");
            _startRoot.gameObject.SetActive(false);
            _gameOverRoot.gameObject.SetActive(false);
            _root.gameObject.SetActive(false);
        }
    }
}
