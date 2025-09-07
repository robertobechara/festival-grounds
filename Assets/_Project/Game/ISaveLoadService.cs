namespace FestivalGrounds.Game
{
    public interface ISaveLoadService
    {
        void SaveGame();
        void LoadGame();
        bool HasSaveFile();
    }
}