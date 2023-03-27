using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefaultNamespace;
using UnityEngine;
using Grid = DefaultNamespace.Grid;


public static class Utilities
{
    
    public static IEnumerator AnimatePotentialMatches(IEnumerable<GameObject> potentialMatches)
    {
        for (float i = 1f; i >= 0.3f; i -= 0.1f)
        {
            foreach (var item in potentialMatches)
            {
                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = i;
                item.GetComponent<SpriteRenderer>().color = c;
            }
            yield return new WaitForSeconds(Constants.OpacityAnimationFrameDelay);
        }
        for (float i = 0.3f; i <= 1f; i += 0.1f)
        {
            foreach (var item in potentialMatches)
            {
                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = i;
                item.GetComponent<SpriteRenderer>().color = c;
            }
            yield return new WaitForSeconds(Constants.OpacityAnimationFrameDelay);
        }
    }
    public static bool AreNeighbors(Grid s1, Grid s2)
    {
        return Mathf.Abs(s1.Row - s2.Row) <= 1
               && Mathf.Abs(s1.Column - s2.Column) <= 1
               && (s1.Row == s2.Row || s1.Column == s2.Column
                                    || Mathf.Abs(s1.Row - s2.Row) == Mathf.Abs(s1.Column - s2.Column));
    }
    public static IEnumerable<GameObject> GetPotentialMatches(GridsArray grids,GridManager g)
    {
        //list that will contain all the matches we find
        List<List<GameObject>> matches = new List<List<GameObject>>();
       
        for (int row = 0; row < g.row; row++)
        {
            for (int column = 0; column < g.col; column++)
            {

                var matches1 = CheckHorizontal1(row, column, grids,g);
                var matches2 = CheckHorizontal2(row, column, grids,g);
                var matches3 = CheckHorizontal3(row, column, grids,g);
                var matches4 = CheckVertical1(row, column, grids,g);
                var matches5 = CheckVertical2(row, column, grids,g);
                var matches6 = CheckVertical3(row, column, grids,g);
                var matches7 = CheckDiagonal1(row, column, grids,g);
                var matches8 = CheckDiagonal2(row, column, grids,g);
                var matches9 = CheckDiagonal3(row, column, grids,g);

                if (matches1 != null) matches.Add(matches1);
                if (matches2 != null) matches.Add(matches2);
                if (matches3 != null) matches.Add(matches3);
                if (matches4 != null) matches.Add(matches4);
                if (matches5 != null) matches.Add(matches5);
                if (matches6 != null) matches.Add(matches6);
                if (matches7 != null) matches.Add(matches7);
                if (matches8 != null) matches.Add(matches8);
                if (matches9 != null) matches.Add(matches9);

                //if we have >= 3 matches, return a random one
                if (matches.Count >= 3)
                    return matches[UnityEngine.Random.Range(0, matches.Count - 1)];

                //if we are in the middle of the calculations/loops
                //and we have less than 3 matches, return a random one
                if(row >= g.row / 2 && matches.Count > 0 && matches.Count <=2)
                    return matches[UnityEngine.Random.Range(0, matches.Count - 1)];
            }
        }
        return null;
    }
    private static List<GameObject> CheckHorizontal1(int row, int column, GridsArray grids,GridManager g)
    {
        if (column <= g.col - 2)
        {
            if (grids[row, column].GetComponent<Grid>().
                IsSameType(grids[row, column + 1].GetComponent<Grid>()))
            {
                if (row >= 1 && column >= 1)
                    if (grids[row, column].GetComponent<Grid>().
                    IsSameType(grids[row - 1, column - 1].GetComponent<Grid>()))
                        return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row, column + 1].gameObject,
                                    grids[row - 1, column - 1].gameObject
                                };

                /* example *\
                 * * * * *
                 * * * * *
                 * * * * *
                 * & & * *
                 & * * * *
                \* example  */

                if (row <= g.row - 2 && column >= 1)
                    if (grids[row, column].GetComponent<Grid>().
                    IsSameType(grids[row + 1, column - 1].GetComponent<Grid>()))
                        return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row, column + 1].gameObject,
                                    grids[row + 1, column - 1].gameObject
                                };

                /* example *\
                 * * * * *
                 * * * * *
                 & * * * *
                 * & & * *
                 * * * * *
                \* example  */
            }
        }
        return null;
    }
    private static List<GameObject> CheckHorizontal2(int row, int column, GridsArray grids,GridManager g)
    {
        if (column <= g.col - 3)
        {
            if (grids[row, column].GetComponent<Grid>().IsSameType(grids[row, column + 1].GetComponent<Grid>()))
            {

                if (row >= 1 && column <= g.col - 3)
                    if (grids[row, column].GetComponent<Grid>().
                    IsSameType(grids[row - 1, column + 2].GetComponent<Grid>()))
                        return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row, column + 1].gameObject,
                                    grids[row - 1, column + 2].gameObject
                                };

                /* example *\
                 * * * * *
                 * * * * *
                 * * * * *
                 * & & * *
                 * * * & *
                \* example  */

                if (row <= g.row - 2 && column <= g.col - 3)
                    if (grids[row, column].GetComponent<Grid>().
                    IsSameType(grids[row + 1, column + 2].GetComponent<Grid>()))
                        return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row, column + 1].gameObject,
                                    grids[row + 1, column + 2].gameObject
                                };

                /* example *\
                 * * * * *
                 * * * * *
                 * * * & *
                 * & & * *
                 * * * * *
                \* example  */
            }
        }
        return null;
    }
    private static List<GameObject> CheckHorizontal3(int row, int column, GridsArray grids,GridManager g )
    {
        if (column <= g.col - 4)
        {
            if (grids[row, column].GetComponent<Grid>().
               IsSameType(grids[row, column + 1].GetComponent<Grid>()) &&
               grids[row, column].GetComponent<Grid>().
               IsSameType(grids[row, column + 3].GetComponent<Grid>()))
            {
                return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row, column + 1].gameObject,
                                    grids[row, column + 3].gameObject
                                };
            }

            /* example *\
              * * * * *  
              * * * * *
              * * * * *
              * & & * &
              * * * * *
            \* example  */
        }
        if (column >= 2 && column <= g.col - 2)
        {
            if (grids[row, column].GetComponent<Grid>().
               IsSameType(grids[row, column + 1].GetComponent<Grid>()) &&
               grids[row, column].GetComponent<Grid>().
               IsSameType(grids[row, column - 2].GetComponent<Grid>()))
            {
                return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row, column + 1].gameObject,
                                    grids[row, column -2].gameObject
                                };
            }

            /* example *\
              * * * * * 
              * * * * *
              * * * * *
              * & * & &
              * * * * *
            \* example  */
        }
        return null;
    }
    private static List<GameObject> CheckVertical1(int row, int column, GridsArray grids,GridManager g)
    {
        if (row <= g.row - 2)
        {
            if (grids[row, column].GetComponent<Grid>().
                IsSameType(grids[row + 1, column].GetComponent<Grid>()))
            {
                if (column >= 1 && row >= 1)
                    if (grids[row, column].GetComponent<Grid>().
                    IsSameType(grids[row - 1, column - 1].GetComponent<Grid>()))
                        return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row + 1, column].gameObject,
                                    grids[row - 1, column -1].gameObject
                                };

                /* example *\
                  * * * * *
                  * * * * *
                  * & * * *
                  * & * * *
                  & * * * *
                \* example  */

                if (column <= g.col - 2 && row >= 1)
                    if (grids[row, column].GetComponent<Grid>().
                    IsSameType(grids[row - 1, column + 1].GetComponent<Grid>()))
                        return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row + 1, column].gameObject,
                                    grids[row - 1, column + 1].gameObject
                                };

                /* example *\
                  * * * * *
                  * * * * *
                  * & * * *
                  * & * * *
                  * * & * *
                \* example  */
            }
        }
        return null;
    }
    private static List<GameObject> CheckVertical2(int row, int column, GridsArray grids,GridManager g)
    {
        if (row <= g.row - 3)
        {
            if (grids[row, column].GetComponent<Grid>().
                IsSameType(grids[row + 1, column].GetComponent<Grid>()))
            {
                if (column >= 1)
                    if (grids[row, column].GetComponent<Grid>().
                    IsSameType(grids[row + 2, column - 1].GetComponent<Grid>()))
                        return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row + 1, column].gameObject,
                                    grids[row + 2, column -1].gameObject
                                };

                /* example *\
                  * * * * *
                  & * * * *
                  * & * * *
                  * & * * *
                  * * * * *
                \* example  */

                if (column <= g.col - 2)
                    if (grids[row, column].GetComponent<Grid>().
                    IsSameType(grids[row + 2, column + 1].GetComponent<Grid>()))
                        return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row+1, column].gameObject,
                                    grids[row + 2, column + 1].gameObject
                                };

                /* example *\
                  * * * * *
                  * * & * *
                  * & * * *
                  * & * * *
                  * * * * *
                \* example  */

            }
        }
        return null;
    }
    private static List<GameObject> CheckVertical3(int row, int column, GridsArray grids,GridManager g)
    {
        if (row <= g.row - 4)
        {
            if (grids[row, column].GetComponent<Grid>().
               IsSameType(grids[row + 1, column].GetComponent<Grid>()) &&
               grids[row, column].GetComponent<Grid>().
               IsSameType(grids[row + 3, column].GetComponent<Grid>()))
            {
                return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row + 1, column].gameObject,
                                    grids[row + 3, column].gameObject
                                };
            }
        }

        /* example *\
          * & * * *
          * * * * *
          * & * * *
          * & * * *
          * * * * *
        \* example  */

        if (row >= 2 && row <= g.row - 2)
        {
            if (grids[row, column].GetComponent<Grid>().
               IsSameType(grids[row + 1, column].GetComponent<Grid>()) &&
               grids[row, column].GetComponent<Grid>().
               IsSameType(grids[row - 2, column].GetComponent<Grid>()))
            {
                return new List<GameObject>()
                                {
                                    grids[row, column].gameObject,
                                    grids[row + 1, column].gameObject,
                                    grids[row - 2, column].gameObject
                                };
            }
        }

        /* example *\
          * * * * *
          * & * * *
          * & * * *
          * * * * *
          * & * * *
        \* example  */
        return null;
    }
    private static List<GameObject> CheckDiagonal1(int row, int column, GridsArray grids, GridManager g)
{
    if (row <= g.row - 2 && column <= g.col - 2)
    {
        if (grids[row, column].GetComponent<Grid>().
            IsSameType(grids[row + 1, column + 1].GetComponent<Grid>()))
        {
            if (row >= 1 && column >= 1)
                if (grids[row, column].GetComponent<Grid>().
                    IsSameType(grids[row - 1, column - 1].GetComponent<Grid>()))
                    return new List<GameObject>()
                            {
                                grids[row, column].gameObject,
                                grids[row + 1, column + 1].gameObject,
                                grids[row - 1, column - 1].gameObject
                            };

            /* example *\
              * * * * *
              * * & * *
              * * * * *
              * & * * *
              * * * * *
            \* example  */

            if (row >= 1 && column <= g.col - 3)
                if (grids[row, column].GetComponent<Grid>().
                    IsSameType(grids[row - 1, column + 1].GetComponent<Grid>()))
                    return new List<GameObject>()
                            {
                                grids[row, column].gameObject,
                                grids[row + 1, column + 1].gameObject,
                                grids[row - 1, column + 1].gameObject
                            };

            /* example *\
              * * * * *
              * * & * *
              * * * * *
              * * * & *
              * * * * *
            \* example  */
        }
    }
    return null;
}

    private static List<GameObject> CheckDiagonal2(int row, int column, GridsArray grids, GridManager g)
    {
        if (row <= g.row - 3 && column <= g.col - 2)
        {
            if (grids[row, column].GetComponent<Grid>().IsSameType(grids[row + 1, column + 1].GetComponent<Grid>()))
            {
                if (column >= 1)
                    if (grids[row, column].GetComponent<Grid>().IsSameType(grids[row + 2, column - 1].GetComponent<Grid>()))
                        return new List<GameObject>()
                        {
                            grids[row, column].gameObject,
                            grids[row + 1, column + 1].gameObject,
                            grids[row + 2, column - 1].gameObject
                        };

                /* example *\
                  * * * * *
                  * & * * *
                  * * & * *
                  & * * * *
                  * * * * *
                \* example  */

                if (column <= g.col - 3)
                    if (grids[row, column].GetComponent<Grid>().IsSameType(grids[row + 2, column + 2].GetComponent<Grid>()))
                        return new List<GameObject>()
                        {
                            grids[row, column].gameObject,
                            grids[row + 1, column + 1].gameObject,
                            grids[row + 2, column + 2].gameObject
                        };

                /* example *\
                  * * * * *
                  * & * * *
                  * * & * *
                  * * * & *
                  * * * * *
                \* example  */
            }
        }
        return null;
    }

    private static List<GameObject> CheckDiagonal3(int row, int column, GridsArray grids, GridManager g)
    {
        if (row <= g.row - 3 && column <= g.col - 3)
        {
            if (grids[row, column].GetComponent<Grid>().IsSameType(grids[row + 1, column + 1].GetComponent<Grid>()))
            {
                if (grids[row, column].GetComponent<Grid>().IsSameType(grids[row + 2, column + 2].GetComponent<Grid>()))
                    return new List<GameObject>()
                    {
                        grids[row, column].gameObject,
                        grids[row + 1, column + 1].gameObject,
                        grids[row + 2, column + 2].gameObject
                    };

                /* example *\
                  * * * * *
                  * & * * *
                  * * & * *
                  * * * & *
                  * * * * *
                \* example  */
            }
        }
        return null;
    }

}

