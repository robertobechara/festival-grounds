using FestivalGrounds.Data;
using System;

namespace FestivalGrounds.Core
{
    // --- Economy Events ---
    public struct BudgetChangedEvent
    {
        public readonly int NewBudget;
        public BudgetChangedEvent(int newBudget)
        {
            NewBudget = newBudget;
        }
    }

    // --- Build Events ---
    public struct StartBuildModeEvent
    {
        public readonly FacilityConfigSO FacilityConfig;
        public StartBuildModeEvent(FacilityConfigSO facilityConfig)
        {
            FacilityConfig = facilityConfig;
        }
    }

    public struct CancelBuildModeEvent { }

    // --- Game State Events ---
    public struct GameStateChangedEvent
    {
        public readonly GameStateType NewState;
        public GameStateChangedEvent(GameStateType newState)
        {
            NewState = newState;
        }
    }


    // --- Time Events ---
    public struct TimeUpdatedEvent
    {
        public readonly DateTime CurrentTime;
        public TimeUpdatedEvent(DateTime currentTime)
        {
            CurrentTime = currentTime;
        }
    }
        
     public struct TimeScaleChangedEvent
    {
        public readonly float NewTimeScale;
        public TimeScaleChangedEvent(float newTimeScale)
        {
            NewTimeScale = newTimeScale;
        }
    }
}

