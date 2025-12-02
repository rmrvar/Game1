using System;
using System.Collections.Generic;
using Game.RoomSystem;
using Game.Treasure;
using UnityEngine;
using Util;
using Util.TurnSystem;
using Random = UnityEngine.Random;

namespace Game.Character
{
    public class CharacterManager : MonoSingleton<CharacterManager>
    {
        public Action<EnemyCharacter> OnEnemyStartedAction;
        public Action<EnemyCharacter> OnEnemyEndedAction;
        public Action<EnemyCharacter> OnEnemySpawned;
        public Action<EnemyCharacter> OnEnemyKilled;
        public Action<PlayerCharacter> OnPlayerKilled;

        public IEnumerable<EnemyCharacter> Enemies => _enemies;
        public PlayerCharacter Player { get; private set; }
        
        [SerializeField] 
        private PlayerCharacter _playerPrefab;
        [SerializeField] 
        private EnemyCharacter[] _enemyPrefabs;

        public PlayerCharacter SpawnPlayer(TurnManager<Character> turnManager)
        {
            Player = Instantiate(_playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Player.TurnManager = turnManager;
            return Player;
        }
        
        public EnemyCharacter SpawnEnemy(TurnManager<Character> turnManager)
        {
            if (_enemyPrefabs.Length == 0)
            {
                Debug.LogError("Cannot spawn enemy! No enemy prefabs assigned!");
                return null;
            }
            
            var roomEntityPrefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)]
                .GetComponent<RoomEntity>();

            var roomEntity = RoomEntityManager.Instance.SpawnRoomEntity(
                roomEntityPrefab, 
                radius: 3, 
                ignoreOccupiesSpace: false
              );
            var enemy = roomEntity.GetComponent<EnemyCharacter>();
            enemy.TurnManager = turnManager;
            _enemies.Add(enemy);
            OnEnemySpawned?.Invoke(enemy);
            return enemy;
        }

        public void KillEnemy(EnemyCharacter enemy)
        {
            _enemies.Remove(enemy);
            OnEnemyKilled?.Invoke(enemy);
        }

        public void KillPlayer()
        {
            OnPlayerKilled?.Invoke(Player);
        }

        public void ClearAll()
        {
            foreach (var enemy in _enemies)
            {
                Destroy(enemy.gameObject);
            }
            _enemies.Clear();
            if (Player != null)
            {
                Destroy(Player.gameObject);
                Player = null;
            }
        }
        
        private readonly HashSet<EnemyCharacter> _enemies = new();
    }
}
