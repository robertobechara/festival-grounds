namespace FestivalGrounds.Economy
{
    public interface IEconomyService
    {
        int CurrentBudget { get; }
        void AddFunds(int amount);
        bool TrySpend(int amount); 
        void SetBudget(int amount);
    }
}

