using UnityEngine;
using FestivalGrounds.Game;

namespace FestivalGrounds.UI
{
    public class InGameMenu : MonoBehaviour
    {
        public void OnSaveGameButtonClicked()
        {
            GameManager.Instance.SaveGame();
        }
    }
}