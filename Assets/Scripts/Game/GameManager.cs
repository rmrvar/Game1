using UnityEngine;
using Util;

namespace Game
{
    using GameState;
    
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private EGameState _initialGameState;

        public int NumberOfEnemies { get; set; }
        
        public void Start()
        {
            GameStateManager.Instance.SetGameState(_initialGameState);
        }
    }
}
