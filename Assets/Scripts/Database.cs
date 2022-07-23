using System;
using System.Collections.Generic;
using UnityEngine;

public class Database
{
    public readonly Sprite[] IconSprites;
    public readonly List<(string, int)> LeaderBoard;

    private static Database _instance;

    public static Database GetDatabase()
    {
        if (_instance == null)
            _instance = new Database();

        return _instance;
    }

    public bool UpdateLeaderBoard(string date, int score)
    {
        for (int i = 0; i < LeaderBoard.Count; i++)
        {
            if (LeaderBoard[i].Item2 < score)
            {
                LeaderBoard.Insert(i, (date, score));
                PlayerPrefs.SetInt("PlaceInLeaderboard", i);

                if (LeaderBoard.Count > Leaderboard._leaderboardCapacity)
                    LeaderBoard.RemoveAt(_instance.LeaderBoard.Count - 1);

                SaveLeaderboardInPlayerPrefs();

                return true;
            }
        }

        if (LeaderBoard.Count < Leaderboard._leaderboardCapacity)
        {
            LeaderBoard.Add((date, score));
            PlayerPrefs.SetInt("PlaceInLeaderboard", LeaderBoard.Count - 1);
            return true;
        }

            return false;
    }

    private Database()
    {
        IconSprites = Resources.LoadAll<Sprite>("Tile Icons");
        LeaderBoard = new List<(string, int)>();

        if (PlayerPrefs.GetInt("Score1", -1) == -1)
        {
            GetLeaderboardFromCSV();
            SaveLeaderboardInPlayerPrefs();
        }
        else
        {
            GetLeaderboardFromPlayerPrefs();
        }
    }

    private void GetLeaderboardFromCSV()
    {
        string csvFileName = "Leaderboard";
        TextAsset file = Resources.Load<TextAsset>(csvFileName);
        string[] stringRows = file.text.Split('\n');

        for (int i = 1; i < stringRows.Length; i++)
        {
            if (LeaderBoard.Count >= Leaderboard._leaderboardCapacity)
                break;

            string[] data = stringRows[i].Split(',');
            LeaderBoard.Add((data[0], Int32.Parse(data[1])));
        }
        SortLeaderboard();
    }

    private void SortLeaderboard()
    {
        for (int i = 0; i < LeaderBoard.Count - 1; i++)
        {
            for (int j = i; j + 1 > 0; j--)
            {
                if ((LeaderBoard[j].Item2 < LeaderBoard[j + 1].Item2) ||
                   ((LeaderBoard[j].Item2 == LeaderBoard[j + 1].Item2) &&
                     (DateTime.Compare(DateTime.Parse(LeaderBoard[j].Item1).Date, DateTime.Parse(LeaderBoard[j + 1].Item1).Date) > 0)))
                {
                    var temp = LeaderBoard[j];
                    LeaderBoard[j] = LeaderBoard[j + 1];
                    LeaderBoard[j + 1] = temp;
                }
            }
        }
    }

    private void SaveLeaderboardInPlayerPrefs()
    {
        for (int i = 0; i < LeaderBoard.Count; i++)
        {
            PlayerPrefs.SetString($"Date{i + 1}", LeaderBoard[i].Item1);
            PlayerPrefs.SetInt($"Score{i + 1}", LeaderBoard[i].Item2);
        }

        PlayerPrefs.Save();
    }

    private void GetLeaderboardFromPlayerPrefs()
    {
        int i = 1;

        while (true)
        {
            string date = PlayerPrefs.GetString($"Date{i}", "");
            if (date == "")
                break;

            int score = PlayerPrefs.GetInt($"Score{i}", -1);
            if (score == -1)
                break;

            LeaderBoard.Add((date, score));

            i++;
        }
    }
}
