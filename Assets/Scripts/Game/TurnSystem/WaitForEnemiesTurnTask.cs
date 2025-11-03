using System.Collections;
using System.Threading;
using Game.Character;
using UnityEngine;
using Util.TurnSystem;

namespace Game.TurnSystem
{
    public class WaitForEnemiesTurnTask : ITurnTask<Character.Character>
    {
        public IEnumerator IE_Execute(TurnContext<Character.Character> context, CancellationToken token)
        {
            yield return new WaitUntil(() => _count <= 0);
            _count = 0;
        }

        public void OnEnemyStartedAction(EnemyCharacter enemy)
        {
            ++_count;
        }

        public void OnEnemyEndedAction(EnemyCharacter enemy)
        {
            --_count;
        }
        
        private int _count = 0;
    }
}
