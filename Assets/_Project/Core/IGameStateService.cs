    namespace FestivalGrounds.Core
    {
        public interface IGameStateService
        {
            GameStateType CurrentState { get; }
            void SetState(GameStateType newState);
        }
    }
    
