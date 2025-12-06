namespace ExamTwo.Interfaces
{
    public interface ICoffeMachineRepository
    {
        Dictionary<string, int> GetCoffeeStock();
        void UpdateCoffeeStock(string type, int newQuantity);
        Dictionary<string, int> GetCoffeePrices();
        Dictionary<int, int> GetCoinInventory();

        bool UpdateCoffeeStatus(string name, int count);

        bool UpdateCoinStauts(int coin, int count);
    }
}
