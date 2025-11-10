using System;
using System.Collections.Generic;
using Game.RoomSystem;
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

            var prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];

            var positionBot = Vector2Int.zero;
            var positionTop = Vector2Int.zero;
            do
            {
                var p = Random.insideUnitCircle * 3;
                positionBot.x = Mathf.RoundToInt(p.x);
                positionBot.y = Mathf.RoundToInt(p.y);
                positionTop = new Vector2Int(positionBot.x, positionBot.y + 12);
            // Since Room only knows about the bottom colliders, check there. But the room entities are spawning in the 
            // top, so check that there.
            } while (Room.Instance.IsCollisionAt(positionBot) || Room.Instance.GetRoomEntityAt(positionTop)?.OccupiesSpace == true);
            
            var enemy = Instantiate(prefab, new Vector3(positionTop.x, positionTop.y), Quaternion.identity);
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
        
        private HashSet<EnemyCharacter> _enemies = new();
    }
}
