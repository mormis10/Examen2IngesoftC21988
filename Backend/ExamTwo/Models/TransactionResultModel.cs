namespace ExamTwo.Models
{
    public class TransactionResultModel
    {
        public bool Success { get; set; }
        public int ChangeAmount { get; set; }
        public Dictionary<int, int>? ChangeBreakdown { get; set; }
        public string? Message { get; set; }
    }
}
