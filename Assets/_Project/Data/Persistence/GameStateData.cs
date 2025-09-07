using System;
using System.Collections.Generic;

namespace FestivalGrounds.Data
{
    [Serializable]
    public class GameStateData
    {
        public int CurrentBudget;
        public long GameTimeTicks; 
        public int CurrentSims;
        public List<SimData> AllSimsData = new List<SimData>();
        public List<FacilityData> AllFacilitiesData = new List<FacilityData>();
    }
}