using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
    [Flags]
    public enum BonusType
    {
        None,
        DestroyWholeRowColumn
    }
    
    public static class BonusTypeUtilities
    {
        public static bool ContainsDestroyWholeRowColumn(BonusType bt)
        {
            return (bt & BonusType.DestroyWholeRowColumn) 
                   == BonusType.DestroyWholeRowColumn;
        }
    }
    
    public class Grid:MonoBehaviour
    {
       // public BonusType Bonus { get; set; }
        public string GridType { get; set;}
        public int Column { get; set; }
        public int Row { get; set; }

        public Grid()
        {
           // Bonus = BonusType.None;
        }

        public bool IsSameType(Grid otherGrid)
        {
            if (otherGrid == null || !(otherGrid is Grid))
                throw new ArgumentException("otherGrid");
            return otherGrid.GridType == this.GridType;
        }

        public void Assign(string type, int row, int column)
        {
            Column = column;
            Row = row;
            GridType = type;
        }

        public static void SwapColumnRow(Grid a, Grid b)
        {
            int temp = a.Row;
            a.Row = b.Row;
            b.Row = temp;

            temp = a.Column;
            a.Column = b.Column;
            b.Column = temp;
        }
        

    }
}