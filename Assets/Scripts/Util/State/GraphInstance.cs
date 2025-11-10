using System;
using System.Collections.Generic;

namespace Util.State
{
    public class GraphInstance
    {
        public GraphInstance(Graph graph)
        {
            Graph = graph;
        }
        
        public Graph Graph { get; private set; }
        public string CurrentKey { get; private set; }
        public IGraphNode CurrentNode { get; private set; }
        public Dictionary<string, object> Data { get; private set; }

        public void SetState(string key, Dictionary<string, object> args = null)
        {
            if (CurrentNode != null)
            {
                CurrentNode.OnExit(this);
                CurrentNode = null;
            }
            
            if (CurrentKey != null && !Graph.Edges.HasEdge(CurrentKey, key))
            {
                throw new Exception($"Edge ({CurrentKey}->{key}) not found");
            }
            
            var node = Graph.Nodes.GetNode(key);
            if (node == null)
            {
                throw new Exception($"Node {key} not found");
            }

            CurrentNode = node;
            CurrentKey = key;
            
            if (!_initialized.Contains(node))
            {
                node.OnInit(this);
                _initialized.Add(node);
            }
            
            CurrentNode.OnEnter(this, args ?? new Dictionary<string, object>());
        }

        private readonly HashSet<IGraphNode> _initialized = new();
    }
}
