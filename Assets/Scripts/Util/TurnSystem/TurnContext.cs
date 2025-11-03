using UnityEngine;

namespace Util.TurnSystem
{
    public struct TurnContext<T>
    {
        public T Token;
        public MonoBehaviour CoroutineRunner;
    }
}
