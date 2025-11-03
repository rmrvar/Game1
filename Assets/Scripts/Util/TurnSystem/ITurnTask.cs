using System.Collections;
using System.Threading;

namespace Util.TurnSystem
{
    public interface ITurnTask<T>
    {
        bool ShouldSkip() => false;
        IEnumerator IE_Execute(TurnContext<T> context, CancellationToken token);
    }
}
