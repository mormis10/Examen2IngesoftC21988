using NUnit.Framework;
using Moq;
using ExamTwo.Services;
using ExamTwo.Interfaces;
using ExamTwo.Models;
using System.Collections.Generic;

namespace TestExamen2.Services
{
    [TestFixture]
    public class TransactionServiceTests
    {
        private Mock<ICoffeeService> coffeeServiceMock;
        private Mock<IMachineMoneyService> moneyServiceMock;
        private Mock<ICoffeMachineRepository> repositoryMock;
        private TransactionService service;

        [SetUp]
        public void Setup()
        {
            coffeeServiceMock = new Mock<ICoffeeService>();
            moneyServiceMock = new Mock<IMachineMoneyService>();
            repositoryMock = new Mock<ICoffeMachineRepository>();

            service = new TransactionService(
                coffeeServiceMock.Object,
                moneyServiceMock.Object,
                repositoryMock.Object
            );
        }

      
        [Test]
        public void ProcessOrder_ShouldFail_WhenOrderIsEmpty()
        {
            var request = new OrderRequestModel
            {
                Order = new Dictionary<string, int>(),
                Payment = new PaymentModel { TotalAmount = 1000 }
            };

            var result = service.ProcessOrder(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Order is empty.", result.Message);
        }

       
        [Test]
        public void ProcessOrder_ShouldFail_WhenPaymentInvalid()
        {
            var request = new OrderRequestModel
            {
                Order = new Dictionary<string, int> { { "Latte", 1 } },
                Payment = new PaymentModel { TotalAmount = 0 }
            };

            var result = service.ProcessOrder(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Invalid payment.", result.Message);
        }

        [Test]
        public void ProcessOrder_ShouldFail_WhenCoffeeNotFound()
        {
            coffeeServiceMock.Setup(s => s.GetCoffeeStock()).Returns(new Dictionary<string, int>());
            coffeeServiceMock.Setup(s => s.GetCoffeePrices()).Returns(new Dictionary<string, int>());

            var request = new OrderRequestModel
            {
                Order = new Dictionary<string, int> { { "Mocha", 1 } },
                Payment = new PaymentModel { TotalAmount = 2000 }
            };

            var result = service.ProcessOrder(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Coffee 'Mocha' not found.", result.Message);
        }

    
        [Test]
        public void ProcessOrder_ShouldFail_WhenInsufficientFunds()
        {
            coffeeServiceMock.Setup(s => s.GetCoffeeStock())
                .Returns(new Dictionary<string, int> { { "Latte", 5 } });

            coffeeServiceMock.Setup(s => s.GetCoffeePrices())
                .Returns(new Dictionary<string, int> { { "Latte", 1500 } });

            var request = new OrderRequestModel
            {
                Order = new Dictionary<string, int> { { "Latte", 1 } },
                Payment = new PaymentModel { TotalAmount = 500 }
            };

            var result = service.ProcessOrder(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Insufficient funds.", result.Message);
        }

        [Test]
        public void ProcessOrder_ShouldFail_WhenStockInsufficient()
        {
            coffeeServiceMock.Setup(s => s.GetCoffeeStock())
                .Returns(new Dictionary<string, int> { { "Latte", 1 } });

            coffeeServiceMock.Setup(s => s.GetCoffeePrices())
                .Returns(new Dictionary<string, int> { { "Latte", 1000 } });

            var request = new OrderRequestModel
            {
                Order = new Dictionary<string, int> { { "Latte", 2 } },
                Payment = new PaymentModel { TotalAmount = 5000 }
            };

            var result = service.ProcessOrder(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Not enough Latte available.", result.Message);
        }

 
        [Test]
        public void ProcessOrder_ShouldFail_WhenNotEnoughChange()
        {
            coffeeServiceMock.Setup(s => s.GetCoffeeStock())
                .Returns(new Dictionary<string, int> { { "Latte", 2 } });

            coffeeServiceMock.Setup(s => s.GetCoffeePrices())
                .Returns(new Dictionary<string, int> { { "Latte", 1000 } });

            moneyServiceMock.Setup(s => s.GetCoinInventory())
                .Returns(new Dictionary<int, int> { { 500, 0 }, { 100, 0 } });

            var request = new OrderRequestModel
            {
                Order = new Dictionary<string, int> { { "Latte", 1 } },
                Payment = new PaymentModel { TotalAmount = 2000 }
            };

            var result = service.ProcessOrder(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Not enough change in the machine.", result.Message);
        }

        [Test]
        public void ProcessOrder_ShouldSucceed_WhenEverythingIsValid()
        {
            coffeeServiceMock.Setup(s => s.GetCoffeeStock())
                .Returns(new Dictionary<string, int> { { "Latte", 10 } });

            coffeeServiceMock.Setup(s => s.GetCoffeePrices())
                .Returns(new Dictionary<string, int> { { "Latte", 1000 } });

            moneyServiceMock.Setup(s => s.GetCoinInventory())
                .Returns(new Dictionary<int, int> { { 500, 5 }, { 100, 5 } });

            var request = new OrderRequestModel
            {
                Order = new Dictionary<string, int> { { "Latte", 1 } },
                Payment = new PaymentModel { TotalAmount = 2000 }
            };

            var result = service.ProcessOrder(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1000, result.ChangeAmount);
            Assert.IsTrue(result.ChangeBreakdown.ContainsKey(500));
            Assert.AreEqual("Purchase successful.", result.Message);
        }
    }
}
