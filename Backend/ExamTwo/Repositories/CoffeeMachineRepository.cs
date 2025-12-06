using ExamTwo.Controllers;
using ExamTwo.Interfaces;

namespace ExamTwo.Repositories
{
    public class CoffeeMachineRepository : ICoffeMachineRepository
    {
        private readonly Database? database;

        public CoffeeMachineRepository(Database? database)
        {
            this.database = database;
        }

        public Dictionary<string, int> GetCoffeeStock()
        {
            return this.database.getAllCoffeeData();
        }

        public Dictionary<string, int> GetCoffeePrices()
        {
            return this.database.getCofeePrices();
        }

        public void UpdateCoffeeStock(string type, int newQuantity)
        {

        }

        public Dictionary<int, int> GetCoinInventory()
        {
            return this.database.getChangeQuantity();
        }



        public bool UpdateCoffeeStatus(string name, int count)
        {
            return this.database.UpdateCoffeeStatus(name, count);
        }

        public bool UpdateCoinStauts(int coin, int count)
        {
            return this.database.UpdateCoinStatus(coin, count);

        }

    }
}
