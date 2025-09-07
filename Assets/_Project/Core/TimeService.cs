using System;
using UnityEngine;

namespace FestivalGrounds.Core
{
    public class TimeService : ITimeService
    {
        public float DeltaTime { get; private set; }
        
        public DateTime CurrentTime { get; private set; }
        public bool IsPaused => _gameStateService.CurrentState != GameStateType.PlayMode;
        public float CurrentTimeScale => _timeScale;

        private readonly EventBus _eventBus;
        private readonly IGameStateService _gameStateService;
        
        private float _timeScale = 1f;
        private int _lastMinute;

        public TimeService(EventBus eventBus, IGameStateService gameStateService)
        {
            _eventBus = eventBus;
            _gameStateService = gameStateService;
            CurrentTime = new DateTime(2025, 6, 1, 8, 0, 0);
            _lastMinute = CurrentTime.Minute;
        }

        public void Tick(float realDeltaTime)
        {
            if (IsPaused)
            {
                DeltaTime = 0; // When not in PlayMode, our custom time does not advance.
                return;
            }

            // Calculate our custom delta time based on the speed multiplier.
            DeltaTime = realDeltaTime * _timeScale;
            
            // Advance the simulation clock (e.g., 1 real second = 1 game hour at 1x speed)
            double scaledSeconds = DeltaTime * 60; 
            CurrentTime = CurrentTime.AddSeconds(scaledSeconds);

            if (CurrentTime.Minute != _lastMinute)
            {
                _lastMinute = CurrentTime.Minute;
                _eventBus.Publish(new TimeUpdatedEvent(CurrentTime));
            }
        }
        
        public void Pause()
        {
             // We don't control time directly; we ask the GameStateService to change the state.
            if (_gameStateService.CurrentState == GameStateType.PlayMode)
            {
                _gameStateService.SetState(GameStateType.Paused);
            }
        }

        public void Resume()
        {
            if (_gameStateService.CurrentState == GameStateType.Paused)
            {
                _gameStateService.SetState(GameStateType.PlayMode);
            }
        }

        public void SetTimeScale(float scale)
        {
            // This now only affects our internal multiplier, not the global engine time.
            _timeScale = Mathf.Max(0, scale);
            // Broadcast the change to any system that is listening.
            _eventBus.Publish(new TimeScaleChangedEvent(_timeScale));
        }

        public void SetTime(DateTime time)
        {
            CurrentTime = time;
            _lastMinute = CurrentTime.Minute;
            _eventBus.Publish(new TimeUpdatedEvent(CurrentTime));
        }
    }
}