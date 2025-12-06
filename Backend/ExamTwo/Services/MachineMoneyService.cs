using ExamTwo.Interfaces;

namespace ExamTwo.Services
{
    public class MachineMoneyService : IMachineMoneyService
    {
     private readonly ICoffeMachineRepository coffeMachineRepository;

     public MachineMoneyService(ICoffeMachineRepository coffeMachineRepository)
       {
            this.coffeMachineRepository = coffeMachineRepository;
       }

      public Dictionary<int, int> GetCoinInventory()
        {
            return coffeMachineRepository.GetCoinInventory();
        }

      public bool UpdateCoins(int coin, int count)
        {
            return this.coffeMachineRepository.UpdateCoinStauts(coin, count);
        }
    }
}
