using Game.Common;
using Game.GameState;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.Ui
{
    public class MenuUi : MonoBehaviour
    {
        [SerializeField]
        private bool _isGameOver;
        
        public void OnStartButtonClicked()
        {
            if (_isGameOver)
            {
                // This is a hack.
                var roomState = GameStateManager.Instance.StateMachine.Graph.Nodes.GetNode(Constants.GameState.ROOM_STATE_KEY);
                roomState.OnInit(GameStateManager.Instance.StateMachine);
            }
            GameStateManager.Instance.StateMachine.SetState(
                Constants.GameState.ROOM_STATE_KEY
              );
        }

        public void OnQuitButtonClicked()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
