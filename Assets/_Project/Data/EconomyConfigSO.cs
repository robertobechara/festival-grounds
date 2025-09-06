using UnityEngine;

namespace FestivalGrounds.Data
{
    [CreateAssetMenu(fileName = "EconomyConfig", menuName = "FestivalGrounds/Economy Configuration")]
    public class EconomyConfigSO : ScriptableObject
    {
        [Tooltip("The amount of money the player starts the game with.")]
        public int StartingBudget = 20000;

        [Tooltip("Ticket Price")]
        public int TicketPrice = 100;
    }
}

