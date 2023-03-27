using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class MatchesInfo
    {
        private List<GameObject> _matchedGrids;
        
        public IEnumerable<GameObject> MatchedGrids => _matchedGrids.Distinct();

        private void AddGrids(GameObject g)
        {
            if(!_matchedGrids.Contains(g))
                _matchedGrids.Add(g);
        }

        public void AddGridRange(IEnumerable<GameObject> grids)
        {
            foreach (var i in grids)
            {
                AddGrids(i);
            }
        }

        public MatchesInfo()
        {
            _matchedGrids = new List<GameObject>();
        }
    }
}