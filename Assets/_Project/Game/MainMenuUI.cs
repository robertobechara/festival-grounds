
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using FestivalGrounds.Game;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _loadGameButton;

    private void Start()
    {
        // Check if a save file exists and enable/disable the load button accordingly.
        string savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        if (_loadGameButton != null)
        {
            _loadGameButton.interactable = File.Exists(savePath);
        }
    }

    public void OnNewGameButtonClicked()
    {
        GameManager.Instance.StartNewGame();
    }

    public void OnLoadGameButtonClicked()
    {
        GameManager.Instance.LoadSavedGame();
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