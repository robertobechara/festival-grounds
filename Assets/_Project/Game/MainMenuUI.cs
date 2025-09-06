
using UnityEngine;
using FestivalGrounds.Game;

public class MainMenuUI : MonoBehaviour
{
    public void OnNewGameButtonClicked()
    {
        GameManager.Instance.StartNewGame();
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}