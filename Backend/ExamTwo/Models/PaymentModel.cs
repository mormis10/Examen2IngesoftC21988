namespace ExamTwo.Models
{
    public class PaymentModel
    {
        public int TotalAmount { get; set; }
        public List<int>? Coins { get; set; }
        public List<int>? Bills { get; set; }
    }
}
