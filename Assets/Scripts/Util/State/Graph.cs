using System;

namespace Util.State
{
    public class Graph
    {
        public IImmutableGraphNodes Nodes { get; }
        public IImmutableGraphEdges Edges { get; }

        public class Builder
        {
            public Graph Graph { get; private set; }

            public static Builder Start()
            {
                return new Builder();
            }

            public Builder AddNode(string key, IGraphNode node)
            {
                if (Graph != null)
                {
                    throw new InvalidOperationException("Builder already finished!");
                }
                _nodes.AddNode(key, node);
                return this;
            }
            
            public Builder AddEdge(string fromKey, string toKey)
            {
                if (Graph != null)
                {
                    throw new InvalidOperationException($"Builder already finished!");
                }
                _edges.AddEdge(fromKey, toKey);
                return this;
            }

            public Graph Finish()
            {
                Graph = new Graph(_nodes, _edges);
                return Graph;
            }

            private Builder()
            {
            }
            
            private readonly GraphEdges _edges = new();
            private readonly GraphNodes _nodes = new();
        }
    
        private Graph(IImmutableGraphNodes nodes, IImmutableGraphEdges edges)
        {
            Nodes = nodes;
            Edges = edges;
        }
    }
}