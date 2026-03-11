using System.Collections;
using System.Threading;
using Util.TurnSystem;

namespace Game.TurnSystem
{
    public class TurnTask : ITurnTask<Character.Character>
    {
        public IEnumerator IE_Execute(TurnContext<Character.Character> context, CancellationToken token)
        {
            yield return context.Token.IE_ExecuteTurn(token);
        }
    }
}