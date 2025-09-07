using FestivalGrounds.Core;
using FestivalGrounds.Economy;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FestivalGrounds.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Main HUD")]
        [SerializeField] private TextMeshProUGUI _budgetDisplay;
        [SerializeField] private TextMeshProUGUI _timeDisplayText;

        [Header("State-Driven UI")]
        [SerializeField] private GameObject _buildToolbarPanel;
        [SerializeField] private Button _modeToggleButton;
        [SerializeField] private TextMeshProUGUI _modeButtonText;
        [Header("Time Controls")]
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _fastForwardButton;

        [Header("Time Button Visuals")]
        [SerializeField] private Color _activeColor = new Color(1f, 0.8f, 0.4f);
        [SerializeField] private Color _inactiveColor = Color.white;

        private EventBus _eventBus;
        private IGameStateService _gameStateService;
        private DateTime _festivalStartDate;
        private bool _startDateSet = false;

        private void Awake()
        {
            _eventBus = ServiceLocator.GetService<EventBus>();
            _gameStateService = ServiceLocator.GetService<IGameStateService>();

            _eventBus.Subscribe<BudgetChangedEvent>(OnBudgetChanged);
            _eventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
            _eventBus.Subscribe<TimeUpdatedEvent>(OnTimeUpdated);
            _eventBus.Subscribe<TimeScaleChangedEvent>(OnTimeScaleChanged);
        }

        private void Start()
        {
            // Proactively get the starting time and update the display
            var timeService = ServiceLocator.GetService<ITimeService>();
            OnTimeUpdated(new TimeUpdatedEvent(timeService.CurrentTime));
            OnTimeScaleChanged(new TimeScaleChangedEvent(timeService.CurrentTimeScale));

            // Proactively get the starting budget and update the display
            var economyService = ServiceLocator.GetService<IEconomyService>();
            OnBudgetChanged(new BudgetChangedEvent(economyService.CurrentBudget));

            // Proactively get the starting game state and update the UI
            OnGameStateChanged(new GameStateChangedEvent(_gameStateService.CurrentState));
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<BudgetChangedEvent>(OnBudgetChanged);
                _eventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
                _eventBus.Unsubscribe<TimeUpdatedEvent>(OnTimeUpdated);
                _eventBus.Unsubscribe<TimeScaleChangedEvent>(OnTimeScaleChanged);
            }
        }

        private void OnBudgetChanged(BudgetChangedEvent e)
        {
            if (_budgetDisplay != null)
            {
                _budgetDisplay.text = $"${e.NewBudget:N0}";
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            _buildToolbarPanel.SetActive(e.NewState == GameStateType.BuildMode);

            bool isBuilding = e.NewState == GameStateType.BuildMode;
            _modeButtonText.text = isBuilding ? "Play" : "Build";

            _pauseButton.interactable = !isBuilding;
            _playButton.interactable = !isBuilding;
            _fastForwardButton.interactable = !isBuilding;
        }

        private void OnTimeUpdated(TimeUpdatedEvent e)
        {
            if (!_startDateSet)
            {
                _festivalStartDate = e.CurrentTime;
                _startDateSet = true;
            }

            TimeSpan elapsedTime = e.CurrentTime - _festivalStartDate;
            int dayNumber = elapsedTime.Days + 1;

            _timeDisplayText.text = $"Day {dayNumber} {e.CurrentTime:HH:mm}";
        }

        private void OnTimeScaleChanged(TimeScaleChangedEvent e)
        {
            Image pauseImg = _pauseButton.GetComponent<Image>();
            Image playImg = _playButton.GetComponent<Image>();
            Image ffImg = _fastForwardButton.GetComponent<Image>();

            pauseImg.color = _inactiveColor;
            playImg.color = _inactiveColor;
            ffImg.color = _inactiveColor;

            if (e.NewTimeScale == 0f)
            {
                pauseImg.color = _activeColor;
            }
            else if (e.NewTimeScale == 1f)
            {
                playImg.color = _activeColor;
            }
            else // Any other speed (2x, 4x, etc.) highlights the fast-forward button
            {
                ffImg.color = _activeColor;
            }
        }

        public void OnModeToggleButtonClicked()
        {
            var currentState = _gameStateService.CurrentState;
            if (currentState == GameStateType.BuildMode)
            {
                _gameStateService.SetState(GameStateType.PlayMode);
            }
            else if (currentState == GameStateType.PlayMode)
            {
                _gameStateService.SetState(GameStateType.BuildMode);
            }
        }

        // Public method of the Pause/Play buttons' OnClick events
        public void SetTimeScale(float scale)
        {
            var timeService = ServiceLocator.GetService<ITimeService>();
            timeService.SetTimeScale(scale);
        }

        // Public method for the Fast Forward button's OnClick event
        public void OnFastForwardClicked()
        {
            var timeService = ServiceLocator.GetService<ITimeService>();
            float currentScale = timeService.CurrentTimeScale;
            float newScale;

            if (currentScale == 1f || currentScale == 0f)
            {
                newScale = 2f;
            }
            else if (currentScale == 2f)
            {
                newScale = 4f;
            }
            else
            {
                newScale = 1;
            }
            
            timeService.SetTimeScale(newScale);
        }
    }
}

