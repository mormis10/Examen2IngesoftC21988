using ExamTwo.Interfaces;

namespace ExamTwo.Services
{
    public class CoffeeService : ICoffeeService
    {
        private readonly ICoffeMachineRepository coffeMachineRepository;

        public CoffeeService(ICoffeMachineRepository service)
        {
            this.coffeMachineRepository = service;
        }
        public Dictionary<string, int> GetCoffeeStock()
        {
            return this.coffeMachineRepository.GetCoffeeStock();
        }

        public Dictionary<string, int> GetCoffeePrices()
        {
            return this.coffeMachineRepository.GetCoffeePrices();
        }
        public bool UpdateCoffePrices(string coffeeName, int count)
        {
            return this.coffeMachineRepository.UpdateCoffeeStatus(coffeeName, count);

        }
    }
}
