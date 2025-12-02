using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Game.Common;
using Game.Misc;
using Game.RoomSystem;
using Game.Treasure;
using Game.TurnSystem;
using Game.Ui;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;
using Util.State;
using Util.TurnSystem;

namespace Game.GameState
{
    public class RoomGameState : MonoSingletonState<RoomGameState>
    {
        [SerializeField] 
        private PlayerCharacter _playerPrefab;

        [SerializeField]
        private Vector3 _camPos1;
        [SerializeField]
        private Vector3 _camPos2;
        [SerializeField]
        private Camera _camera;
        
        [SerializeField]
        private Tilemap _botTilemap1;
        [SerializeField] 
        private Tilemap _botTilemap2;
        [SerializeField]
        private Tilemap _topTilemap;

        [SerializeField]
        private Gate _topGate;
        [SerializeField] 
        private Gate _botGate;

        [SerializeField] 
        private HealthUi _healthUi;
        
        public override void OnInit(GraphInstance instance)
        {
            Debug.Log("RoomGameState OnInit");
        }
        
        public override void OnEnter(GraphInstance instance, Dictionary<string, object> args)
        {
            base.OnEnter(instance, args);
            Debug.Log("RoomGameState OnEnter");

            if (CharacterManager.Instance.Player == null)
            {
                TreasureManager.Instance.Reset();
                _level = 1;
                _turnManager = new TurnManager<Character.Character>(this);
                SetBottomTilemap(true);
                _camera.transform.position = _camPos1;
                _player = CharacterManager.Instance.SpawnPlayer(_turnManager);
                _turnManager.AddToken(_player);
                _healthUi.Health = _player.Health;
                _playerTasks = new ITurnTask<Character.Character>[]
                {
                    new WaitForEnemiesTurnTask(),
                    new TurnTask(),
                };
                
                _topGate.Lower();
                _botGate.Lower();
            }
            
            CharacterManager.Instance.OnEnemyStartedAction +=
                ((WaitForEnemiesTurnTask)_playerTasks[0]).OnEnemyStartedAction;
            CharacterManager.Instance.OnEnemyEndedAction += 
                ((WaitForEnemiesTurnTask)_playerTasks[0]).OnEnemyEndedAction;
            CharacterManager.Instance.OnEnemyKilled += OnEnemyKilled;
            CharacterManager.Instance.OnPlayerKilled += OnPlayerKilled;
            _turnManager.OnTurnStarted += OnTurnStarted;
                
            _turnManager.Run();
        }

        public override void OnExit(GraphInstance instance)
        {            
            CharacterManager.Instance.OnEnemyStartedAction -=
                ((WaitForEnemiesTurnTask)_playerTasks[0]).OnEnemyStartedAction;
            CharacterManager.Instance.OnEnemyEndedAction -= 
                ((WaitForEnemiesTurnTask)_playerTasks[0]).OnEnemyEndedAction;
            CharacterManager.Instance.OnEnemyKilled -= OnEnemyKilled;
            CharacterManager.Instance.OnPlayerKilled -= OnPlayerKilled;
            _turnManager.OnTurnStarted -= OnTurnStarted;
            
            _turnManager.Stop();
        }

        private void OnEnemyKilled(EnemyCharacter enemy)
        {
            if (!CharacterManager.Instance.Enemies.Any())
            {
                _topGate.Lower();   
            }
        }

        private void OnPlayerKilled(PlayerCharacter player)
        {
            CharacterManager.Instance.ClearAll();
            GameStateManager.Instance.StateMachine.SetState(
                Constants.GameState.MENU_STATE_KEY,
                new Dictionary<string, object>()
                {
                    { Constants.MenuState.SUBSTATE_ARG_NAME, Constants.MenuState.GAMEOVER_SUBSTATE_KEY }      
                }
              );
        }

        public int RoomTransitionThresholdY => (int) _topGate.transform.position.y;
        
        public IEnumerator IE_DoRoomTransition()
        {
            ++_level;
            
            SpawnEnemies();
            
            // Play animations.
            int completionCounter = 0;
            _player.RoomEntity.Move(EDirection.UP, 0.5F, (_) => ++completionCounter);   
            StartCoroutine(LerpAnimation.IE_MoveTo(
                _camera.transform, 
                _camPos2, 
                0.35F, 
                (_) => ++completionCounter)
              );
            yield return new WaitUntil(() => completionCounter == 2);
            
            TreasureManager.Instance.ClearTreasures();
            
            if (Room.Instance.Tilemap == _botTilemap1)
            {
                SetBottomTilemap(useBotTilemap1: false);
            }
            
            // Teleport player & enemies down.
            _player.RoomEntity.Position = new Vector2Int(_player.RoomEntity.Position.x, _player.RoomEntity.Position.y - 12);
            foreach (var enemy in CharacterManager.Instance.Enemies)
            {
                enemy.RoomEntity.Position = new Vector2Int(enemy.RoomEntity.Position.x, enemy.RoomEntity.Position.y - 12);
            }
            
            // Teleport camera back down.
            _camera.transform.position = _camPos1;
            
            // Trap the player.
            _topGate.Raise();
            _botGate.Raise();
            
            // This is usually calculated when the turns end (which was in the top room).
            Dijkstra.SetDestination(_player.RoomEntity.Position);
            
            // Reset player delta time so they don't teleport.
            PlayerInput.Instance.SetDeltaTimeForMove(0);
            
            _turnManager.Stop();
            _turnManager.Run();
        }

        private int GetNumOfEnemies()
        {
            switch (_level)
            {
                case 1:
                case 2: 
                    return Random.Range(4, 5);
                case 3: 
                case 4: 
                    return Random.Range(5, 6);
                case 5: 
                case 6: 
                    return Random.Range(7, 8);
                case 7: 
                case 8: 
                    return Random.Range(9, 10);
                default: 
                    return Random.Range(11, 15);
            }
        }
        
        private void SpawnEnemies()
        {
            var enemies = new List<EnemyCharacter>();
            for (int i = 0; i < GetNumOfEnemies(); ++i)
            {
                var enemy = CharacterManager.Instance.SpawnEnemy(_turnManager);
                enemies.Add(enemy);    
            }
            foreach (var enemy in enemies)
            {
                _turnManager.AddToken(enemy);
            }
        }

        private void SetBottomTilemap(bool useBotTilemap1)
        {
            _botTilemap1.gameObject.SetActive(useBotTilemap1);
            _botTilemap2.gameObject.SetActive(!useBotTilemap1);
            Room.Instance.SetTilemap(useBotTilemap1 ? _botTilemap1 : _botTilemap2);
        }

        private void OnTurnStarted(TurnContext<Character.Character> context)
        {
            if (context.Token is PlayerCharacter)
            {
                _turnManager.SetTasks(_playerTasks);
            } else 
            if (context.Token is EnemyCharacter)
            {
                _turnManager.SetTasks(_enemyTasks);    
            }
            else
            {
                Debug.LogError("Invalid token in TurnContext!");
            }
        }
        
        private ITurnTask<Character.Character>[] _playerTasks;
        private readonly ITurnTask<Character.Character>[] _enemyTasks = 
        {
            new TurnTask(), 
        };

        private TurnManager<Character.Character> _turnManager;
        private PlayerCharacter _player;
        private int _level;
    }
}
