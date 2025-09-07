using FestivalGrounds.Core;

namespace FestivalGrounds.Economy
{
    public class EconomyService : IEconomyService
    {
        private int _currentBudget;
        private readonly EventBus _eventBus;

        // This public property replaces the GetCurrentBudget() method to satisfy the interface.
        public int CurrentBudget => _currentBudget;

        // This is the correct constructor that GameManager is trying to call.
        public EconomyService(int startingBudget, EventBus eventBus)
        {
            _currentBudget = startingBudget;
            _eventBus = eventBus;
            
            // Announce the initial budget when the service is created.
            _eventBus.Publish(new BudgetChangedEvent(_currentBudget));
        }

        // This new method satisfies the IEconomyService interface requirement.
        public void AddFunds(int amount)
        {
            if (amount > 0)
            {
                _currentBudget += amount;
                _eventBus.Publish(new BudgetChangedEvent(_currentBudget));
            }
        }

        public bool TrySpend(int amount)
        {
            if (_currentBudget >= amount)
            {
                _currentBudget -= amount;
                _eventBus.Publish(new BudgetChangedEvent(_currentBudget));
                return true;
            }

            return false;
        }

        public void SetBudget(int amount)
        {
            _currentBudget = amount;
            _eventBus.Publish(new BudgetChangedEvent(_currentBudget));
        }
    }
}

