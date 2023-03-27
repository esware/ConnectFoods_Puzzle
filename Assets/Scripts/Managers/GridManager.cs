using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using DefaultNamespace;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Grid = DefaultNamespace.Grid;
using Random = UnityEngine.Random;

public enum GridState
{
    None,
    SelectionStarted,
    Animating
}

public class GridManager : MonoBehaviour
{
    [System.Serializable]
    public struct Goals
    {
        public int[] goalsCount;
        public GameObject[] goalsObjects;
        public GameObject defaultGrid;
 
        public bool IsGoalCompleted()
        {
            foreach (var t in goalsCount)
            {
                if (t > 1)
                {
                    return false;
                }
            }

            return true;
        }
        public void InitGoals(Transform goalsTransform)
        {
            newObjects = new List<GameObject>(goalsObjects.Length);
            for (int i = 0; i < goalsObjects.Length; i++)
            {
                GameObject newObject = Instantiate(defaultGrid,Vector3.zero, Quaternion.identity, goalsTransform);
                newObject.GetComponent<Image>().sprite = 
                    goalsObjects[i].GetComponent<SpriteRenderer>().sprite;
                newObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    goalsCount[i].ToString();
                
                newObjects.Add(newObject.gameObject);

            }

            foreach (var c in newObjects)
            {
                Debug.Log(c);
            }
        }
        public void UpdateGoals()
        {
            if (newObjects != null && newObjects.Count > 0)
            {
                for (int i = 0; i < newObjects.Count; i++)
                {
                    if (goalsCount[i] <1)
                    {
                        newObjects[i].transform.GetChild(0).gameObject.SetActive(false);
                        Color tempColor = newObjects[i].GetComponent<Image>().color;
                        tempColor.a = 0.5f;
                        newObjects[i].GetComponent<Image>().color = tempColor;
                    }
                    else
                    {
                        newObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = goalsCount[i].ToString();
                    }
                }
            }
            else
            {
                Debug.Log("_newObjects is null or empty!");
            }
        }
    }
    
    [HideInInspector] public static GridManager Instance;
    [SerializeField]  public int maxMoves;
    public int leftMoves => _leftMoves;
    public static List<GameObject> newObjects = new List<GameObject>();
    public Goals goals;
    public int row;
    public int col;
    public GameObject[] gridPrefabs;
    public GameObject[] explosionPrefabs;

    private readonly Vector2 _bottomRight = new Vector2(0, 0);
    private readonly Vector2 _candySize = new Vector2(.95f, .95f);
    private GridState _state = GridState.None;
    private GameObject _hitGo = null;
    private GridsArray _grids;
    private Vector2[] _spawnPositions;
    private IEnumerator _checkPotentialMatchesCoroutine;
    private IEnumerator _animatePotentialMatchesCoroutine;
    private IEnumerable<GameObject> _potentialMatches;
    private int _leftMoves;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Time.timeScale = 0.8f;
        _leftMoves = maxMoves;
        InitializeTypesOnPrefabShapesAndBonuses();
        InitializeCandyAndSpawnPositions();
        StartCheckForPotentialMatches();
        SignUpEvents();
    }
    private void SignUpEvents()
    {
        GameEvents.LoseEvent += (() => _state = GridState.None);
    }
    private void Update()
    {
        if (_state == GridState.None)
        {
            //user has clicked or touched
            if (Input.GetMouseButtonDown(0))
            {
                //get the hit position
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null) //we have a hit!!!
                {
                    _hitGo = hit.collider.gameObject;
                    _state = GridState.SelectionStarted;
                }
                
            }
        }
        else if (_state == GridState.SelectionStarted)
        {
            //user dragged
            if (Input.GetMouseButton(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                //we have a hit
                if (hit.collider != null && _hitGo != hit.collider.gameObject)
                {
                    //user did a hit, no need to show him hints 
                    StopCheckForPotentialMatches();

                    //if the two shapes are diagonally aligned (different row and column), just return
                    if (!Utilities.AreNeighbors(_hitGo.GetComponent<Grid>(),hit.collider.gameObject.GetComponent<Grid>()))
                    {
                        _state = GridState.None;
                    }
                    else
                    {
                        _state = GridState.Animating;
                        FixSortingLayer(_hitGo, hit.collider.gameObject);
                        StartCoroutine(FindMatchesAndCollapse(hit));
                    }
                }
            }
        }
    }
    private void InitializeTypesOnPrefabShapesAndBonuses()
    {
        //just assign the name of the prefab
        foreach (var item in gridPrefabs)
        {
            item.GetComponent<Grid>().GridType = item.name;

        }
    }
    private void FixSortingLayer(GameObject hitGo, GameObject hitGo2)
    {
        SpriteRenderer sp1 = hitGo.GetComponent<SpriteRenderer>();
        SpriteRenderer sp2 = hitGo2.GetComponent<SpriteRenderer>();
        if (sp1.sortingOrder <= sp2.sortingOrder)
        {
            sp1.sortingOrder = 1;
            sp2.sortingOrder = 0;
        }
    }
    private IEnumerator FindMatchesAndCollapse(RaycastHit2D hit2)
    {
        //get the second item that was part of the swipe
        var hitGo2 = hit2.collider.gameObject;
        _grids.Swap(_hitGo, hitGo2);

        //move the swapped ones
        _hitGo.transform.positionTo(Constants.AnimationDuration, hitGo2.transform.position);
        hitGo2.transform.positionTo(Constants.AnimationDuration, _hitGo.transform.position);
        yield return new WaitForSeconds(Constants.AnimationDuration);

        //get the matches via the helper methods
        var hitGomatchesInfo = _grids.GetMatches(_hitGo);
        var hitGo2matchesInfo = _grids.GetMatches(hitGo2);

        var totalMatches = hitGomatchesInfo.MatchedGrids
            .Union(hitGo2matchesInfo.MatchedGrids).Distinct();

        //if user's swap didn't create at least a 3-match, undo their swap
        if (totalMatches.Count() < Constants.MinimumMatches)
        {
            _hitGo.transform.positionTo(Constants.AnimationDuration, hitGo2.transform.position);
            hitGo2.transform.positionTo(Constants.AnimationDuration, _hitGo.transform.position);
            yield return new WaitForSeconds(Constants.AnimationDuration);

            _grids.UndoSwap();
        }
        else
        {
            _leftMoves -= 1;
        }
        int timesRun = 1;
        while (totalMatches.Count() >= Constants.MinimumMatches)
        {
            //increase score
            IncreaseScore((totalMatches.Count() - 2) * Constants.Match3Score);

            if (timesRun >= 2)
                IncreaseScore(Constants.SubsequentMatchScore);

            //soundManager.PlayCrincle();

            foreach (var item in totalMatches)
            {
                var i = item.GetComponent<Grid>();

                for (int j = 0; j < goals.goalsObjects.Length; j++)
                {
                    if (i.GridType == goals.goalsObjects[j].gameObject.name)
                    {
                        goals.goalsCount[j]--;
                    }
                }
                
                _grids.Remove(item);
                RemoveFromScene(item);
                goals.UpdateGoals();
            }
            
            var columns = totalMatches.Select(go => go.GetComponent<Grid>().Column).Distinct();
            
            var collapsedCandyInfo = _grids.Collapse(columns);
            //create new ones
            var newCandyInfo = CreateNewCandyInSpecificColumns(columns);

            int maxDistance = Mathf.Max(collapsedCandyInfo.MaxDistance, newCandyInfo.MaxDistance);

            MoveAndAnimate(newCandyInfo.AlteredGrid, maxDistance);
            MoveAndAnimate(collapsedCandyInfo.AlteredGrid, maxDistance);
            
            //will wait for both of the above animations
            yield return new WaitForSeconds(Constants.MoveAnimationMinDuration * maxDistance);

            //search if there are matches with the new/collapsed items
            totalMatches = _grids.GetMatches(collapsedCandyInfo.AlteredGrid).
                Union(_grids.GetMatches(newCandyInfo.AlteredGrid)).Distinct();
            timesRun++;
        }

        if (goals.IsGoalCompleted() && totalMatches.Count() <1)
        {
            UIManager.Instance.ShowWinPanel();
        }
        if (_leftMoves <1&& totalMatches.Count() <1)
        {
            UIManager.Instance.ShowLosePanel();
        }

        _state = GridState.None;
        StartCheckForPotentialMatches();
    }
    private void MoveAndAnimate(IEnumerable<GameObject> movedGameObjects, int distance)
     {
         foreach (var item in movedGameObjects)
         {
             item.transform.positionTo(Constants.MoveAnimationMinDuration * distance, _bottomRight +
                 new Vector2(item.GetComponent<Grid>().Column * _candySize.x, item.GetComponent<Grid>().Row * _candySize.y));
         }
     }
    private AlteredGridInfo CreateNewCandyInSpecificColumns(IEnumerable<int> columnsWithMissingCandy)
    {
        AlteredGridInfo newCandyInfo = new AlteredGridInfo();

        //find how many null values the column has
        foreach (int column in columnsWithMissingCandy)
        {
            var emptyItems = _grids.GetEmptyItemsOnColumn(column,this);
            foreach (var item in emptyItems)
            {
                var go = GetRandomCandy();
                GameObject newCandy = Instantiate(go.gameObject, _spawnPositions[column], Quaternion.identity)
                    as GameObject;

                newCandy.GetComponent<Grid>().Assign(go.GetComponent<Grid>().GridType, item.Row, item.Column);

                if (row - item.Row > newCandyInfo.MaxDistance)
                    newCandyInfo.MaxDistance = row - item.Row;

                _grids[item.Row, item.Column] = newCandy;
                newCandyInfo.AddCandy(newCandy);
            }
        }
        return newCandyInfo;
    }
    private void RemoveFromScene(GameObject item)
     {
         GameObject explosion = GetRandomExplosion();
         var newExplosion = Instantiate(explosion, item.transform.position, Quaternion.identity) as GameObject;
         Destroy(newExplosion, Constants.ExplosionDuration);
         Destroy(item);
     }
    private GameObject GetRandomExplosion()
     {
         return explosionPrefabs[Random.Range(0, explosionPrefabs.Length)];
     }
    private void InitializeVariables()
    {
        ShowScore();
    }
    private void IncreaseScore(int amount)
    {
        ShowScore();
    }
    private void ShowScore()
    {
       // ScoreText.text = "Score: " + score.ToString();
    }
    private void DestroyAllCandy()
    {
        for (int row = 0; row < col; row++)
        {
            for (int column = 0; column < col; column++)
            {
                Destroy(_grids[row, column]);
            }
        }
    }
    private void InitializeCandyAndSpawnPositions()
    {
        InitializeVariables();

        if (_grids != null)
            DestroyAllCandy();

        _grids = new GridsArray(row,col);
        _spawnPositions = new Vector2[col];

        for (int row = 0; row < this.row; row++)
        {
            for (int column = 0; column < col; column++)
            {
                var randomCandy = GetRandomCandy();

                while (IsHorizontalMatch(row, column, randomCandy) || IsVerticalMatch(row, column, randomCandy) || IsDiagonalMatch(row, column, randomCandy))
                {
                    randomCandy = GetRandomCandy();
                }

                InstantiateAndPlaceNewCandy(row, column, randomCandy.gameObject);
            }
        }

        SetupSpawnPositions();
    }

    private bool IsHorizontalMatch(int row, int column, GameObject newCandy)
    {
        return column >= 2 && _grids[row, column - 1].GetComponent<Grid>().IsSameType(newCandy.GetComponent<Grid>()) && _grids[row, column - 2].GetComponent<Grid>().IsSameType(newCandy.GetComponent<Grid>());
    }

    private bool IsVerticalMatch(int row, int column, GameObject newCandy)
    {
        return row >= 2 && _grids[row - 1, column].GetComponent<Grid>().IsSameType(newCandy.GetComponent<Grid>()) && _grids[row - 2, column].GetComponent<Grid>().IsSameType(newCandy.GetComponent<Grid>());
    }

    private bool IsDiagonalMatch(int row, int column, GameObject newCandy)
    {
        return (row >= 2 && column >= 2 && _grids[row - 1, column - 1].GetComponent<Grid>().IsSameType(newCandy.GetComponent<Grid>()) && _grids[row - 2, column - 2].GetComponent<Grid>().IsSameType(newCandy.GetComponent<Grid>())) ||
               (row >= 2 && column < col - 2 && _grids[row - 1, column + 1].GetComponent<Grid>().IsSameType(newCandy.GetComponent<Grid>()) && _grids[row - 2, column + 2].GetComponent<Grid>().IsSameType(newCandy.GetComponent<Grid>()));
    }

    private void InstantiateAndPlaceNewCandy(int row, int column, GameObject newCandy)
    {
        var go = Instantiate(newCandy, _bottomRight + new Vector2(column * _candySize.x, row * _candySize.y), Quaternion.identity);
        //assign the specific properties
        go.GetComponent<Grid>().Assign(newCandy.GetComponent<Grid>().GridType, row, column);
        _grids[row, column] = go;
    }
    private void SetupSpawnPositions()
    {
        //create the spawn positions for the new shapes (will pop from the 'ceiling')
        for (int column = 0; column < col; column++)
        {
            _spawnPositions[column] = _bottomRight
                                     + new Vector2(column * _candySize.x, row * _candySize.y);
        }
    }
    private GameObject GetRandomCandy()
    {
        return gridPrefabs[Random.Range(0, gridPrefabs.Length)];
    }
    private void StartCheckForPotentialMatches()
    {
        StopCheckForPotentialMatches();
        //get a reference to stop it later
        _checkPotentialMatchesCoroutine = CheckPotentialMatches();
        StartCoroutine(_checkPotentialMatchesCoroutine);
    }
    private void StopCheckForPotentialMatches()
    {
        if (_animatePotentialMatchesCoroutine != null)
            StopCoroutine(_animatePotentialMatchesCoroutine);
        if (_checkPotentialMatchesCoroutine != null)
            StopCoroutine(_checkPotentialMatchesCoroutine);
        ResetOpacityOnPotentialMatches();
    }
    private void ResetOpacityOnPotentialMatches()
    {
        if (_potentialMatches != null)
            foreach (var item in _potentialMatches)
            {
                if (item == null) break;

                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = 1.0f;
                item.GetComponent<SpriteRenderer>().color = c;
            }
    }
    private IEnumerator CheckPotentialMatches()
    {
        yield return new WaitForSeconds(Constants.WaitBeforePotentialMatchesCheck);
        _potentialMatches = Utilities.GetPotentialMatches(_grids,this);
        if (_potentialMatches != null)
        {
            while (true)
            {

                _animatePotentialMatchesCoroutine = Utilities.AnimatePotentialMatches(_potentialMatches);
                StartCoroutine(_animatePotentialMatchesCoroutine);
                yield return new WaitForSeconds(Constants.WaitBeforePotentialMatchesCheck);
            }
        }
    }

}


