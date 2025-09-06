
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
        // This will run in a standalone build
        #if UNITY_STANDALONE
            Application.Quit();
        #endif

            // This will run in the Unity Editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}