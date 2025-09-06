using FestivalGrounds.Core;
using System.Collections.Generic;
using UnityEngine;

namespace FestivalGrounds.Facilities
{
    public class QueueHandler : MonoBehaviour
    {
        // This array is no longer set in the Inspector. It is populated at runtime.
        private Transform[] _queueSpots;

        // The queue now correctly stores the ISimControllable interface, not the concrete SimController.
        private readonly Queue<ISimControllable> _waitingQueue = new Queue<ISimControllable>();

        public bool HasSpace => _queueSpots != null && _waitingQueue.Count < _queueSpots.Length;

        // This method is called by the facility coordinator (e.g., Toilet.cs) after
        // the spots have been procedurally generated.
        public void Initialize(Transform[] spots)
        {
            _queueSpots = spots;
        }

        // Called by a facility when a Sim needs to get in line.
        public Transform Enqueue(ISimControllable sim)
        {
            if (!HasSpace) return null;

            _waitingQueue.Enqueue(sim);
            // The Sim is assigned a spot based on its new position in the queue.
            return _queueSpots[_waitingQueue.Count - 1];
        }

        // Called by a facility when a user spot opens up.
        public ISimControllable Dequeue()
        {
            if (_waitingQueue.Count == 0) return null;
            
            // Get the Sim at the front of the line.
            var leavingSim = _waitingQueue.Dequeue();

            // Tell all remaining Sims to move up one spot.
            UpdateQueuePositions();

            return leavingSim;
        }
        
        // This method tells each Sim in the queue their new spot transform.
        private void UpdateQueuePositions()
        {
            int i = 0;
            foreach (ISimControllable sim in _waitingQueue)
            {
                // The ISimControllable interface has a PromoteFromQueue method,
                // which the SimController implements. This tells the Sim to move.
                sim.PromoteFromQueue(_queueSpots[i]);
                i++;
            }
        }
    }
}