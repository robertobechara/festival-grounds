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

        private static GameManager _instance;
        private IGameStateService _gameStateService;
        private ITimeService _timeService;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeServices();
        }

        private void Start()
        {
            LoadScenes();
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

        private void LoadScenes()
        {
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                SceneManager.LoadScene(i, LoadSceneMode.Additive);
            }
        }
    }
}

