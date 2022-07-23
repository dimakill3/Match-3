using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameProcess : MonoBehaviour
{
    public Action<int> StepChange;
    public Action<int> ScoreChange;
    public Action GameOver;

    [SerializeField] public UnityEvent OnGetIntoLeaderboard;

    [SerializeField] private Board _board;
    [SerializeField] [Range(10, 20)] private int _steps;
    [SerializeField] private TouchHand _template;

    private TouchHand _createdTouchHand;
    private int _score = 0;
    private List<Tile> _matchedTiles;
    private List<Tile> _firstStep;
    private bool _isTutorial;

    private void Awake()
    {
        _board.Initialize();
        _matchedTiles = new List<Tile>();
        _firstStep = new List<Tile>();
    }

    private void OnEnable()
    {
        foreach (IReadOnlyList<Tile> tilesRow in _board.AllTiles)
        {
            foreach (Tile tile in tilesRow)
            {
                tile.OnClick += TapOnTile;
            }
        }
    }

    private void Start()
    {
        StepChange?.Invoke(_steps);
        ScoreChange?.Invoke(_score);
        StartTutorial();
    }

    private void StartTutorial()
    {
        _isTutorial = true;
        FindFirstStep();
        ShowGameplay();
    }

    private void FindFirstStep()
    {
        int previousCount = 0;

        // Ищем максимальный набор совпадающих тайлов
        for (int x = 0; x < _board.Width; x++)
        {
            for (int y = 0; y < _board.Height; y++)
            {
                if (!_matchedTiles.Contains(_board.AllTiles[x][y]))
                {
                    previousCount = _matchedTiles.Count;
                    FindAllMatches(x, y);
                    if (_matchedTiles.Count - previousCount > 2 && _matchedTiles.Count - previousCount > _firstStep.Count)
                        _firstStep = _matchedTiles.GetRange(previousCount, _matchedTiles.Count - previousCount);
                }
            }
        }

        // Если не найден набор тайлов, выбирается случайный тайл
        if (_firstStep.Count == 0)
        {
            int randX = UnityEngine.Random.Range(0, _board.Width - 1);
            int randY = UnityEngine.Random.Range(0, _board.Height - 1);
            _firstStep.Add(_board.AllTiles[randX][randY]);
        }

        // Найденный ход подсвечивается
        foreach (Tile tile in _firstStep)
        {
            tile.TutorialHighlight();
        }
        _matchedTiles.Clear();
    }

    private void ShowGameplay()
    {
        _createdTouchHand = Instantiate(_template);
        _createdTouchHand.MoveTouchHand(_firstStep[0].transform.position.x, _firstStep[0].transform.position.y);
    }

    private void TapOnTile(int xPos, int yPos)
    {
        if (_isTutorial)
        {
            if (CanTouchHere(xPos, yPos) == false)
                return;
        }

        FindAllMatches(xPos, yPos);

        HandleMatches();
    }

    private bool CanTouchHere(int xPos, int yPos)
    {
        if (_firstStep.Contains(_board.AllTiles[xPos][yPos]))
        {
            _isTutorial = false;
            Destroy(_createdTouchHand.gameObject);
            return true;
        }
        else
            return false;

    }

    private void FindAllMatches(int xPos, int yPos)
    {
        _matchedTiles.Add(_board.AllTiles[xPos][yPos]);

        if (xPos > 0)
        {
            Tile left = _board.AllTiles[xPos - 1][yPos];
            if (left != null && !_matchedTiles.Contains(left) && left.SpriteIndex == _board.AllTiles[xPos][yPos].SpriteIndex)
            {
                FindAllMatches(xPos - 1, yPos);
            }
        }

        if (xPos < _board.Width - 1)
        {
            Tile right = _board.AllTiles[xPos + 1][yPos];
            if (right != null && !_matchedTiles.Contains(right) && right.SpriteIndex == _board.AllTiles[xPos][yPos].SpriteIndex)
            {
                FindAllMatches(xPos + 1, yPos);
            }
        }

        if (yPos > 0)
        {
            Tile dawn = _board.AllTiles[xPos][yPos - 1];
            if (dawn != null && !_matchedTiles.Contains(dawn) && dawn.SpriteIndex == _board.AllTiles[xPos][yPos].SpriteIndex)
            {
                FindAllMatches(xPos, yPos - 1);
            }
        }

        if (yPos < _board.Height - 1)
        {
            Tile up = _board.AllTiles[xPos][yPos + 1];
            if (up != null && !_matchedTiles.Contains(up) && up.SpriteIndex == _board.AllTiles[xPos][yPos].SpriteIndex)
            {
                FindAllMatches(xPos, yPos + 1);
            }
        }
    }

    private void HandleMatches()
    {
        if (_matchedTiles.Count == 0)
            throw new InvalidOperationException();
        else if (_matchedTiles.Count <= 2)
        {
            ChangeScore(1);
            ChangeStep(-1);
            _matchedTiles[0].OnMatched();
        }
        else
        {
            ChangeScore(_matchedTiles.Count + (_matchedTiles.Count - 1));
            ChangeStep(_matchedTiles.Count > 5 ? 4 : _matchedTiles.Count - 1);
            foreach (Tile matchTile in _matchedTiles)
            {
                matchTile.OnMatched();
            }
        }

        _matchedTiles.Clear();
    }

    private void ChangeStep(int stepChange)
    {
        _steps += stepChange;
        StepChange?.Invoke(_steps);

        if (_steps <= 0)
            CalculateGameResults();
    }

    private void CalculateGameResults() 
    {
        if (Database.GetDatabase().UpdateLeaderBoard(DateTime.Today.Date.ToString("dd.MM.yyyy"), _score) == true)
        {
            OnGetIntoLeaderboard?.Invoke();
            return;
        }

        GameOver?.Invoke();
    }

    private void ChangeScore(int scoreChange)
    {
        _score += scoreChange;
        ScoreChange?.Invoke(_score);
    }

    private void OnDisable()
    {
        foreach (IReadOnlyList<Tile> tilesRow in _board.AllTiles)
        {
            foreach (Tile tile in tilesRow)
            {
                tile.OnClick -= TapOnTile;
            }
        }
    }
}
