using UnityEngine;

namespace FestivalGrounds.Core
{
    // This is the public contract that any "Sim" must follow.
    // Facilities can interact with this interface without needing to know
    // about the specific SimController class.
    public interface ISimControllable
    {
        // A command telling the Sim to move from a queue to a user spot.
        void PromoteFromQueue(Transform targetSpot);

    }
}