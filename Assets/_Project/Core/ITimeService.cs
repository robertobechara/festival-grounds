using System;

namespace FestivalGrounds.Core
{
    public interface ITimeService
    {
        float DeltaTime { get; }   
        DateTime CurrentTime { get; }
        bool IsPaused { get; }
        float CurrentTimeScale { get; }
        
        void Pause();
        void Resume();
        void SetTimeScale(float scale);
        void Tick(float realDeltaTime);

        void SetTime(DateTime time);
    }
}