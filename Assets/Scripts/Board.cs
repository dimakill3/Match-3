using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int Width => _width;
    public int Height => _height;
    public IReadOnlyList<IReadOnlyList<Tile>> AllTiles => _allTiles;

    [SerializeField] [Range(5, 10)] private int _width;
    [SerializeField] [Range(5, 10)] private int _height;
    [SerializeField] private Tile _template;

    private List<List<Tile>> _allTiles = new List<List<Tile>>();

    public void Initialize()
    {
        for (int x = 0; x < _width; x++)
        {
            _allTiles.Add(new List<Tile>());
            for (int y = 0; y < _height; y++)
            {
                var newTile = Instantiate(_template, transform);
                newTile.Initialize(x, y);

                _allTiles[x].Add(newTile);
            }
        }

        ObserveAllTiles();
    }

    public void InitializeFromCSV(string csvFileName)
    {
        TextAsset file = Resources.Load<TextAsset>(csvFileName);
        string[] stringRows = file.text.Split('\n');

        if (stringRows.Length < _width)
        {
            Debug.LogError($"CSV file {csvFileName + ".csv"} has incorrect board size. Correct: Width = {_width}, Height= {_height}");
            throw new ArgumentOutOfRangeException();
        }

        for (int i = 0; i < _width; i++)
        {
            _allTiles.Add(new List<Tile>());
            string[] data = stringRows[i].Split(',');
            if (data.Length < _height)
            {
                Debug.LogError($"CSV file {csvFileName + ".csv"} has incorrect board size. Correct: Width = {_width}, Height= {_height}");
                throw new ArgumentOutOfRangeException();
            }

            for (int j = 0; j < _height; j++)
            {
                var newTile = Instantiate(_template, transform);
                newTile.InitializeWithoutRandom(i, j, Int32.Parse(data[j]));

                _allTiles[i].Add(newTile);
            }
        }

        ObserveAllTiles();  
    }

    private void OnEnable()
    {
        ObserveAllTiles();
    }

    private void ColumnCrash(int xPos, int yPos, Tile tile)
    {
        StartCoroutine(ColumnCrashCoroutine(xPos, yPos, tile));
    }

    private IEnumerator ColumnCrashCoroutine(int xPos, int yPos, Tile tile)
    {
        for (int i = yPos + 1; i < _height; i++)
        {
            _allTiles[xPos][i].ChangePosition(xPos, i - 1);
            _allTiles[xPos][i - 1] = _allTiles[xPos][i];
        }

        tile.Initialize(xPos, _height - 1);
        _allTiles[xPos][_height - 1] = tile;

        yield return new WaitForSeconds(0.4f);   
    }

    private void ObserveAllTiles()
    {
        foreach (Tile tile in GetComponentsInChildren<Tile>())
        {
            tile.OnWanish += ColumnCrash;
        }
    }

    private void OnDisable()
    {
        foreach (Tile tile in GetComponentsInChildren<Tile>())
        {
            tile.OnWanish -= ColumnCrash;
        }
    }
}
