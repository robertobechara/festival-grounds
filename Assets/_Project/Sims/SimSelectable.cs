using UnityEngine;
using FestivalGrounds.Core;

namespace FestivalGrounds.Sims {
        
    public class SimSelectable : SelectableBase
    {
        [SerializeField] private string simName = "Sim";
        // [SerializeField] private NeedsComponent needs; // your existing needs logic component

        public override string DisplayName => simName;

        public override object GetDetailViewModel()
        {
            return new SimViewModel {
                Name = simName,
                Hunger = 100,
                Bladder = 50,
                Sleep = 75,
                Fun = 20
            };
        }
    }
}