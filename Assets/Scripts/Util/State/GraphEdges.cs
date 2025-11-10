using System.Collections.Generic;

namespace Util.State
{
    public interface IImmutableGraphEdges
    {
        bool HasEdge(string fromKey, string toKey);
    }
    
    public class GraphEdges : IImmutableGraphEdges
    {
        public bool HasEdge(string fromKey, string toKey)
        {
            if (fromKey == null || toKey == null)
            {
                return false;
            }
            return _edges.TryGetValue(fromKey, out var toKeys) && toKeys.Contains(toKey);
        }
        
        public void AddEdge(string fromKey, string toKey)
        {
            _edges.TryAdd(fromKey, new HashSet<string>());
            _edges[fromKey].Add(toKey);
        }

        private readonly Dictionary<string, HashSet<string>> _edges = new();
    }
}