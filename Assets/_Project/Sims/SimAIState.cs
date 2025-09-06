namespace FestivalGrounds.Sims
{
    public enum SimAIState
    {
        Spawning,
        NavigatingToTent,
        Wandering,
        SeekingFacility,    // State for moving to the FACILITY'S GENERAL AREA
        NavigatingToSpot,   // NEW state for moving to a specific USER or QUEUE SPOT
        UsingFacility,
        ReturningToTent,
        Sleeping,
        Queueing
    }
}