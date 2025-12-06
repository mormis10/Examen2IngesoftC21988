using ExamTwo.Controllers;
using ExamTwo.Models;

namespace ExamTwo.Interfaces
{
    public interface IMachineMoneyService
    {
        Dictionary<int, int> GetCoinInventory();

        bool UpdateCoins(int coin, int count);
    }
}
