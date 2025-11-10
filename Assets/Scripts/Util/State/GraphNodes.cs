using System;
using System.Collections.Generic;

namespace Util.State
{
    public interface IImmutableGraphNodes
    {
        public bool HasNode(string key);
        public IGraphNode GetNode(string key);
    }
    
    public class GraphNodes : IImmutableGraphNodes
    {
        public bool HasNode(string key)
        {
            return _nodes.ContainsKey(key);
        }
        
        public IGraphNode GetNode(string key)
        {
            return _nodes[key];
        }
        
        public void AddNode(string key, IGraphNode node)
        {
            if (node == null)
            {
                throw new Exception($"Tried to add null key {key}");
            }
            _nodes.Add(key, node);
        }

        private readonly Dictionary<string, IGraphNode> _nodes = new();
    }
}