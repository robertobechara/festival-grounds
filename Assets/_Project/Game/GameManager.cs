using System.Collections; 
using System.Collections.Generic;
using FestivalGrounds.Core;
using FestivalGrounds.Data;
using FestivalGrounds.Economy;
using FestivalGrounds.Facilities;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace FestivalGrounds.Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private EconomyConfigSO _economyConfig;
        
        // Add scene configuration here
        [Header("Scene Management")]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";
        [SerializeField] private List<string> _gameplaySceneNames = new List<string> { "World", "UI" };

        public static GameManager Instance { get; private set; } // Change _instance to a public property
        private IGameStateService _gameStateService;
        private ITimeService _timeService;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this; // Use the public property
            DontDestroyOnLoad(gameObject);

            InitializeServices();
        }

        private void Start()
        {
            SceneManager.LoadScene(_mainMenuSceneName, LoadSceneMode.Additive);
        }

        public void StartNewGame()
        {
            StartCoroutine(LoadGameplayScenes());
        }

        private IEnumerator LoadGameplayScenes()
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

            // 3. Set the "active" scene. 
            // This determines where new objects are instantiated and affects lighting settings.
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_gameplaySceneNames[0])); // e.g., "World"

            Debug.Log("Gameplay scenes loaded.");
            _gameStateService.SetState(GameStateType.BuildMode);
        }

        private void Update()
        {
            _timeService?.Tick(Time.deltaTime);
        }

        private void InitializeServices()
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

    }
}