using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameProcess))]
public class GameUI : MonoBehaviour
{
    public static Action OnGoToMenuModalWindowOpen;
    public static Action OnGoToMenuModalWindowClose;

    [SerializeField] private TMP_Text _textSteps;
    [SerializeField] private TMP_Text _textScore;
    [SerializeField] private Canvas _canvasGoToMenu;
    [SerializeField] private Canvas _canvasGameOver;
    [SerializeField] private Button _buttonToMenu;

    private TouchHand _touchHand;

    public void OpenGoToMenuModalWindow()
    {
        _canvasGoToMenu.gameObject.SetActive(true);
        GetComponent<GameProcess>().enabled = false;
        _buttonToMenu.interactable = false;

        _touchHand = FindObjectOfType<TouchHand>();
        if (_touchHand)
        {
            _touchHand.gameObject.SetActive(false);
        }

        OnGoToMenuModalWindowOpen?.Invoke();
    }

    public void ContinueGame()
    {
        _canvasGoToMenu.gameObject.SetActive(false);
        GetComponent<GameProcess>().enabled = true;
        _buttonToMenu.interactable = true;

        if (_touchHand)
        {
            _touchHand.gameObject.SetActive(true);
        }

        OnGoToMenuModalWindowClose?.Invoke();
    }

    private void OnEnable()
    {
        GameProcess gameProcess = GetComponent<GameProcess>();

        gameProcess.StepChange += UpdateStep;
        gameProcess.ScoreChange += UpdateScore;
        gameProcess.GameOver += OnGameOver;
    }

    private void UpdateStep(int steps)
    {
        _textSteps.text = $"Шаги: {steps}";
    }

    private void UpdateScore(int score)
    {
        _textScore.text = $"Очки: {score}";
    }

    private void OnGameOver()
    {
        _buttonToMenu.interactable = false;
        _canvasGameOver.gameObject.SetActive(true);
        GetComponent<GameProcess>().enabled = false;
    }

    private void OnDisable()
    {
        GameProcess gameProcess = GetComponent<GameProcess>();

        gameProcess.StepChange -= UpdateStep;
        gameProcess.StepChange -= UpdateScore;
        gameProcess.GameOver -= OnGameOver;
    }
}
