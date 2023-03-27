using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class AlteredGridInfo
    {
        private List<GameObject> newGrid { get; set; }
        public int MaxDistance { get; set; }

        public IEnumerable<GameObject> AlteredGrid => newGrid.Distinct();

        public void AddCandy(GameObject go)
        {
            if (!newGrid.Contains(go))
                newGrid.Add(go);
        }
        
        public AlteredGridInfo()
        {
            newGrid = new List<GameObject>();
        }
    }
}