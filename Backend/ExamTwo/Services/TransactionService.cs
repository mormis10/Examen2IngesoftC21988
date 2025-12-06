using ExamTwo.Interfaces;
using ExamTwo.Models;

namespace ExamTwo.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ICoffeeService coffeeService;
        private readonly IMachineMoneyService moneyService;
        private readonly ICoffeMachineRepository repository;

        //En este caso creo que era un buen ejemplo para añadirlo visto en el curso y aplicar dependency injection
        public TransactionService(
            ICoffeeService coffeeService,
            IMachineMoneyService moneyService,
            ICoffeMachineRepository repository)
        {
            this.coffeeService = coffeeService;
            this.moneyService = moneyService;
            this.repository = repository;
        }

        public TransactionResultModel ProcessOrder(OrderRequestModel request)
        {
            var result = new TransactionResultModel();

            if (request.Order == null || request.Order.Count == 0)
            {
                return new TransactionResultModel
                {
                    Success = false,
                    Message = "Order is empty."
                };
            }

            if (request.Payment == null || request.Payment.TotalAmount <= 0)
            {
                return new TransactionResultModel
                {
                    Success = false,
                    Message = "Invalid payment."
                };
            }

            var stock = coffeeService.GetCoffeeStock();         
            var prices = coffeeService.GetCoffeePrices();       
            var coinsInventory = moneyService.GetCoinInventory(); 

          
            int totalCost = 0;

            try
            {
                totalCost = request.Order.Sum(o =>
                {
                    if (!prices.ContainsKey(o.Key))
                        throw new ArgumentException($"Coffee '{o.Key}' not found.");

                    return prices[o.Key] * o.Value;
                });
            }
            catch (Exception ex)
            {
                return new TransactionResultModel
                {
                    Success = false,
                    Message = ex.Message
                };
            }

            if (request.Payment.TotalAmount < totalCost)
            {
                return new TransactionResultModel
                {
                    Success = false,
                    Message = "Insufficient funds."
                };
            }

            foreach (var item in request.Order)
            {
                if (!stock.ContainsKey(item.Key))
                {
                    return new TransactionResultModel
                    {
                        Success = false,
                        Message = $"Coffee '{item.Key}' does not exist."
                    };
                }

                if (item.Value > stock[item.Key])
                {
                    return new TransactionResultModel
                    {
                        Success = false,
                        Message = $"Not enough {item.Key} available."
                    };
                }
            }


            foreach (var item in request.Order)
                stock[item.Key] -= item.Value;

            int change = request.Payment.TotalAmount - totalCost;

            var changeBreakdown = new Dictionary<int, int>();

            foreach (var denom in coinsInventory.Keys.OrderByDescending(c => c))
            {
                int available = coinsInventory[denom];

                int needed = change / denom;

                int use = Math.Min(needed, available);

                if (use > 0)
                {
                    changeBreakdown[denom] = use;
                    change -= denom * use;
                    coinsInventory[denom] -= use;
                }
            }

            if (change > 0)
            {
                return new TransactionResultModel
                {
                    Success = false,
                    Message = "Not enough change in the machine."
                };
            }

            return new TransactionResultModel
            {
                Success = true,
                ChangeAmount = request.Payment.TotalAmount - totalCost,
                ChangeBreakdown = changeBreakdown,
                Message = "Purchase successful."
            };
        }
    }
}
