using System.Collections.Generic;

namespace Util.State
{
    public interface IGraphNode
    {
        void OnInit(GraphInstance instance);
        void OnEnter(GraphInstance instance, Dictionary<string, object> args);
        void OnExit(GraphInstance instance);
    }
}
