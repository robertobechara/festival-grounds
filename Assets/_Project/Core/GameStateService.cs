    namespace FestivalGrounds.Core
    {
        public class GameStateService : IGameStateService
        {
            public GameStateType CurrentState { get; private set; }

            private readonly EventBus _eventBus;

            public GameStateService(EventBus eventBus)
            {
                _eventBus = eventBus;
                CurrentState = GameStateType.Initializing;
            }

            public void SetState(GameStateType newState)
            {
                if (CurrentState == newState) return;

                CurrentState = newState;
                _eventBus.Publish(new GameStateChangedEvent(newState));
                UnityEngine.Debug.Log($"Game State changed to: {newState}");
            }
        }
    }
    
