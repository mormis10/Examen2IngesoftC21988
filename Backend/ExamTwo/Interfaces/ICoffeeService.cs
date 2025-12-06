namespace ExamTwo.Interfaces
{
    public interface ICoffeeService
    {
        Dictionary<string, int> GetCoffeeStock();
        Dictionary<string, int> GetCoffeePrices();

        bool UpdateCoffePrices(string coffeeName, int count);
    }
}
