using System.Collections; 
using System.Collections.Generic;
using FestivalGrounds.Core;
using FestivalGrounds.Data;
using FestivalGrounds.Economy;
using FestivalGrounds.Facilities;
using FestivalGrounds.World;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace FestivalGrounds.Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private EconomyConfigSO _economyConfig;
        [SerializeField] private FacilityDatabaseSO _facilityDatabase;
        [SerializeField] private SimConfigSO _simConfig;
        
        [Header("Core Prefabs")]
        [SerializeField] private GameObject _simPrefab; 
        
        // Add scene configuration here
        [Header("Scene Management")]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";
        [SerializeField] private List<string> _gameplaySceneNames = new List<string> { "World", "UI" };

        public static GameManager Instance { get; private set; }
        private IGameStateService _gameStateService;
        private ITimeService _timeService;
        private ISaveLoadService _saveLoadService;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this; 
            DontDestroyOnLoad(gameObject);

            InitializeCoreServices();
        }

        private void Start()
        {
            SceneManager.LoadScene(_mainMenuSceneName, LoadSceneMode.Additive);
        }

        public void StartNewGame()
        {
            StartCoroutine(LoadGameplayScenes(false));
        }

        public void LoadSavedGame()
        {
            StartCoroutine(LoadGameplayScenes(true));
        }

        public void SaveGame()
        {
            // The GameManager delegates the save command to the service.
            if (_saveLoadService != null)
            {
                _saveLoadService.SaveGame();
            }
            else
            {
                Debug.LogWarning("Cannot save game: SaveLoadService is not yet available.");
            }
        }

        private IEnumerator LoadGameplayScenes(bool loadFromSave)
        {
            // 1. Unload the Main Menu scene.
            Debug.Log("Unloading Main Menu...");
            yield return SceneManager.UnloadSceneAsync(_mainMenuSceneName);

            // 2. Load all the necessary gameplay scenes additively.
            Debug.Log("Loading Gameplay Scenes...");
            foreach (var sceneName in _gameplaySceneNames)
            {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }

            // Wait until the WorldOrganizer has reported that it is ready.
            yield return new WaitUntil(() => WorldOrganizer.Instance != null);

            // 3. Set the "active" scene. 
            // This determines where new objects are instantiated and affects lighting settings.
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_gameplaySceneNames[0])); // e.g., "World"

            // 4. Find object and create SaveLoadService
            InitializeSceneDependentServices();

            // 5. Decide whether to load the save file or start a fresh game.
            if (loadFromSave && _saveLoadService.HasSaveFile())
            {
                Debug.Log("Loading saved game...");
                _saveLoadService.LoadGame();
                _gameStateService.SetState(GameStateType.PlayMode); // Start in play mode when loading
            }
            else
            {
                Debug.Log("Starting a new game...");
                _gameStateService.SetState(GameStateType.BuildMode); // Start in build mode for a new game
            }

        }

        private void Update()
        {
            _timeService?.Tick(Time.deltaTime);
        }

        private void InitializeCoreServices()
        {
            ServiceLocator.Clear();

            var eventBus = new EventBus();
            ServiceLocator.Register(eventBus);

            var gameStateService = new GameStateService(eventBus);
            ServiceLocator.Register<IGameStateService>(gameStateService);
            _gameStateService = gameStateService;

            var timeService = new TimeService(eventBus, gameStateService);
            ServiceLocator.Register<ITimeService>(timeService);
            _timeService = timeService;

            var economyService = new EconomyService(_economyConfig.StartingBudget, eventBus);
            ServiceLocator.Register<IEconomyService>(economyService);

            var facilityTracker = new FacilityTrackerService();
            ServiceLocator.Register<IFacilityTrackerService>(facilityTracker);

            Debug.Log("All core services initialized.");
        }

        private void InitializeSceneDependentServices()
        {
            Transform simsParent = WorldOrganizer.Instance.SimsParent;
            Transform facilitiesParent = WorldOrganizer.Instance.PlacedFacilitiesParent;

            var saveLoadService = new SaveLoadService(_facilityDatabase, _simConfig, _simPrefab, facilitiesParent, simsParent);
            ServiceLocator.Register<ISaveLoadService>(saveLoadService);
            _saveLoadService = saveLoadService;
        }

    }
}