using ExamTwo.Controllers;
using ExamTwo.Models;
namespace ExamTwo.Interfaces
{
    public interface ITransactionService
    {
        TransactionResultModel ProcessOrder(OrderRequestModel request);
    }
}
