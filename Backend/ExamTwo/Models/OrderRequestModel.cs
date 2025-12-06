using ExamTwo.Controllers;

namespace ExamTwo.Models
{
    public class OrderRequestModel
    {
        public Dictionary<string, int>? Order { get; set; }
        public PaymentModel? Payment { get; set; }
    }
}
