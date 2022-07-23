using System.Collections;
using TMPro;
using UnityEngine;

public class LeaderboardRow : MonoBehaviour
{
    [SerializeField] private TMP_Text _place;
    [SerializeField] private TMP_Text _date;
    [SerializeField] private TMP_Text _score;

    public void Initialize(int place, string date, int score)
    {
        _place.text = place.ToString();
        _date.text = date;
        _score.text = score.ToString();
    }

    public void Highlight()
    {
        StartCoroutine(TextChangeColor());
    }

    private IEnumerator TextChangeColor()
    {
        while (true)
        {
            Color color = _place.color;
            for (float red = 0; red <= 1; red += 0.1f)
            {
                color.r = red;
                _place.color = color;
                _date.color = color;
                _score.color = color;
                yield return new WaitForSeconds(0.05f);
            }

            for (float red = 1; red >= 0; red -= 0.1f)
            {
                color.r = red;
                _place.color = color;
                _date.color = color;
                _score.color = color;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
