using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class GridsArray
    {
        private GameObject[,] _grids;
        private GameObject _backupG1;
        private GameObject _backupG2;
        
        public GridsArray(int row,int col)
        {
            _grids = new GameObject[row, col];
        }
        public GameObject this[int row, int column]
        {
            get
            {
                try
                {
                    return _grids[row, column];
                }
                catch (Exception ex)
                {
                
                    throw ex;
                }
            }
            set
            {
                _grids[row, column] = value;
            }
        }
        public void Swap(GameObject g1, GameObject g2)
        {
            _backupG1 = g1;
            _backupG2 = g2;
            
            var grid1 = g1.GetComponent<Grid>();
            var grid2 = g2.GetComponent<Grid>();
            
            int g1Row = grid1.Row;
            int g1Column = grid1.Column;
            int g2Row = grid2.Row;
            int g2Column = grid2.Column;
            
            var temp = _grids[g1Row, g1Column];
            _grids[g1Row, g1Column] = _grids[g2Row, g2Column];
            _grids[g2Row, g2Column] = temp;
            
            Grid.SwapColumnRow(grid1, grid2);
        }
        
        public void UndoSwap()
        {
            if (_backupG1 == null || _backupG2 == null)
                throw new Exception("Backup is null");

            Swap(_backupG1, _backupG2);
        }
        
        public IEnumerable<GameObject> GetMatches(IEnumerable<GameObject> gos)
        {
            List<GameObject> matches = new List<GameObject>();
            foreach (var go in gos)
            {
                matches.AddRange(GetMatches(go).MatchedGrids);
            }
            return matches.Distinct();
        }
        
        public MatchesInfo GetMatches(GameObject go)
        {
            MatchesInfo matchesInfo = new MatchesInfo();

            var horizontalMatches = GetMatchesHorizontally(go);
            matchesInfo.AddGridRange(horizontalMatches);
            var verticalMatches = GetMatchesVertically(go);
            matchesInfo.AddGridRange(verticalMatches);
            var diagonalMatches = GetMatchesDiagonally(go);
            matchesInfo.AddGridRange(diagonalMatches);
            return matchesInfo;
        }
        
        private IEnumerable<GameObject> GetEntireRow(GameObject go)
        {
            List<GameObject> matches = new List<GameObject>();
            int row = go.GetComponent<Grid>().Row;
            for (int column = 0; column < GridManager.Instance.col ; column++)
            {
                matches.Add(_grids[row, column]);
            }
            return matches;
        }
        private IEnumerable<GameObject> GetEntireColumn(GameObject go)
        {
            List<GameObject> matches = new List<GameObject>();
            int column = go.GetComponent<Grid>().Column;
            for (int row = 0; row < GridManager.Instance.row ; row++)
            {
                matches.Add(_grids[row, column].gameObject);
            }
            return matches;
        }
        private IEnumerable<GameObject> GetMatchesHorizontally(GameObject go)
        {
            List<GameObject> matches = new List<GameObject>();
            matches.Add(go);
            var shape = go.GetComponent<Grid>();
            //check left
            if (shape.Column != 0)
                for (int column = shape.Column - 1; column >= 0; column--)
                {
                    if (_grids[shape.Row, column].GetComponent<Grid>().IsSameType(shape))
                    {
                        matches.Add(_grids[shape.Row, column].gameObject);
                    }
                    else
                        break;
                }

            //check right
            if (shape.Column != GridManager.Instance.col  - 1)
                for (int column = shape.Column + 1; column < GridManager.Instance.col ; column++)
                {
                    if (_grids[shape.Row, column].GetComponent<Grid>().IsSameType(shape))
                    {
                        matches.Add(_grids[shape.Row, column].gameObject);
                    }
                    else
                        break;
                }

            //we want more than three matches
            if (matches.Count < Constants.MinimumMatches)
                matches.Clear();

            return matches.Distinct();
        }
        private IEnumerable<GameObject> GetMatchesVertically(GameObject go)
        {
            List<GameObject> matches = new List<GameObject>();
            matches.Add(go);
            var shape = go.GetComponent<Grid>();
            //check bottom
            if (shape.Row != 0)
                for (int row = shape.Row - 1; row >= 0; row--)
                {
                    if (_grids[row, shape.Column] != null &&
                        _grids[row, shape.Column].GetComponent<Grid>().IsSameType(shape))
                    {
                        matches.Add(_grids[row, shape.Column].gameObject);
                    }
                    else
                        break;
                }

            //check top
            if (shape.Row != GridManager.Instance.row  - 1)
                for (int row = shape.Row + 1; row < GridManager.Instance.row ; row++)
                {
                    if (_grids[row, shape.Column] != null && 
                        _grids[row, shape.Column].GetComponent<Grid>().IsSameType(shape))
                    {
                        matches.Add(_grids[row, shape.Column].gameObject);
                    }
                    else
                        break;
                }


            if (matches.Count < Constants.MinimumMatches)
                matches.Clear();

            return matches.Distinct();
        }
         private IEnumerable<GameObject> GetMatchesDiagonally(GameObject go)
{
    List<GameObject> matches = new List<GameObject>();
    matches.Add(go);
    var shape = go.GetComponent<Grid>();

    // check left-up diagonal
    if (shape.Column != 0 && shape.Row != GridManager.Instance.row - 1)
    {
        for (int i = 1; shape.Column - i >= 0 && shape.Row + i < GridManager.Instance.row; i++)
        {
            if (_grids[shape.Row + i, shape.Column - i].GetComponent<Grid>().IsSameType(shape))
            {
                matches.Add(_grids[shape.Row + i, shape.Column - i].gameObject);
            }
            else
            {
                break;
            }
        }
    }

    // check right-down diagonal
    if (shape.Column != GridManager.Instance.col - 1 && shape.Row != 0)
    {
        for (int i = 1; shape.Column + i < GridManager.Instance.col && shape.Row - i >= 0; i++)
        {
            if (_grids[shape.Row - i, shape.Column + i].GetComponent<Grid>().IsSameType(shape))
            {
                matches.Add(_grids[shape.Row - i, shape.Column + i].gameObject);
            }
            else
            {
                break;
            }
        }
    }

    // check left-down diagonal
    if (shape.Column != 0 && shape.Row != 0)
    {
        for (int i = 1; shape.Column - i >= 0 && shape.Row - i >= 0; i++)
        {
            if (_grids[shape.Row - i, shape.Column - i].GetComponent<Grid>().IsSameType(shape))
            {
                matches.Add(_grids[shape.Row - i, shape.Column - i].gameObject);
            }
            else
            {
                break;
            }
        }
    }

    // check right-up diagonal
    if (shape.Column != GridManager.Instance.col - 1 && shape.Row != GridManager.Instance.row - 1)
    {
        for (int i = 1; shape.Column + i < GridManager.Instance.col && shape.Row + i < GridManager.Instance.row; i++)
        {
            if (_grids[shape.Row + i, shape.Column + i].GetComponent<Grid>().IsSameType(shape))
            {
                matches.Add(_grids[shape.Row + i, shape.Column + i].gameObject);
            }
            else
            {
                break;
            }
        }
    }

    if (matches.Count < Constants.MinimumMatches)
    {
        matches.Clear();
    }

    return matches.Distinct();
}
        public void Remove(GameObject item)
        {
            _grids[item.GetComponent<Grid>().Row, item.GetComponent<Grid>().Column] = null;
        }
        
        public AlteredGridInfo Collapse(IEnumerable<int> columns)
        {
            AlteredGridInfo collapseInfo = new AlteredGridInfo();


            ///search in every column
            foreach (var column in columns)
            {
                //begin from bottom row
                for (int row = 0; row < GridManager.Instance.row - 1; row++)
                {
                    //if you find a null item
                    if (_grids[row, column] == null)
                    {
                        //start searching for the first non-null
                        for (int row2 = row + 1; row2 < GridManager.Instance.row ; row2++)
                        {
                            //if you find one, bring it down (i.e. replace it with the null you found)
                            if (_grids[row2, column] != null)
                            {
                                _grids[row, column] = _grids[row2, column];
                                _grids[row2, column] = null;

                                //calculate the biggest distance
                                if (row2 - row > collapseInfo.MaxDistance) 
                                    collapseInfo.MaxDistance = row2 - row;

                                //assign new row and column (name does not change)
                                _grids[row, column].GetComponent<Grid>().Row = row;
                                _grids[row, column].GetComponent<Grid>().Column = column;

                                collapseInfo.AddCandy(_grids[row, column].gameObject);
                                break;
                            }
                        }
                    }
                }
            }

            return collapseInfo;
        }
        
        public IEnumerable<GridInfo> GetEmptyItemsOnColumn(int column,GridManager g)
        {
            List<GridInfo> emptyItems = new List<GridInfo>();
            for (int row = 0; row < g.row; row++)
            {
                if (_grids[row, column] == null)
                    emptyItems.Add(new GridInfo() { Row = row, Column = column });
            }
            return emptyItems;
        }
        
    }
}