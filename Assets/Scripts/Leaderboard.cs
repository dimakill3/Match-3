using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public static int _leaderboardCapacity = 8;
    public int CurrentSize
    {
        get;
        private set;
    } = 0;

    [SerializeField] private LeaderboardRow _template;

    private void Start()
    {
        int placeInLeaderboard = PlayerPrefs.GetInt("PlaceInLeaderboard", -1);
        // Инициализируем таблицу рекордов
        for (int i = 0; i < Database.GetDatabase().LeaderBoard.Count; i++)
        {
            if (i >= _leaderboardCapacity)
                break;

            LeaderboardRow newLeaderboardRow = Instantiate(_template, transform);
            newLeaderboardRow.Initialize(i + 1, Database.GetDatabase().LeaderBoard[i].Item1, Database.GetDatabase().LeaderBoard[i].Item2);
            if (i == placeInLeaderboard)
            {
                newLeaderboardRow.Highlight();
                PlayerPrefs.SetInt("PlaceInLeaderboard", -1);
            }
            
            CurrentSize += 1;
        }
    }
}
