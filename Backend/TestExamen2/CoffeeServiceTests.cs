using NUnit.Framework;
using Moq;
using ExamTwo.Interfaces;
using ExamTwo.Services;
using System.Collections.Generic;
namespace TestExamen2
{
    [TestFixture]
    public class CoffeeServiceTests
    {
        private Mock<ICoffeMachineRepository> _mockRepo;
        private CoffeeService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<ICoffeMachineRepository>();
            _service = new CoffeeService(_mockRepo.Object);
        }

       
        [Test]
        public void GetCoffeeStock_ReturnsRepositoryValues()
        {
            // Arrange
            var expected = new Dictionary<string, int>
            {
                { "Americano", 5 },
                { "Cappuccino", 10 }
            };

            _mockRepo.Setup(r => r.GetCoffeeStock()).Returns(expected);

            // Act
            var result = _service.GetCoffeeStock();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetCoffeePrices_ReturnsRepositoryValues()
        {
            // Arrange
            var expected = new Dictionary<string, int>
            {
                { "Americano", 1000 },
                { "Cappuccino", 1200 }
            };

            _mockRepo.Setup(r => r.GetCoffeePrices()).Returns(expected);

            // Act
            var result = _service.GetCoffeePrices();

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void UpdateCoffePrices_WhenRepoReturnsTrue_ReturnsTrue()
        {
            // Arrange
            string coffeeName = "Americano";
            int count = 2;

            _mockRepo
                .Setup(r => r.UpdateCoffeeStatus(coffeeName, count))
                .Returns(true);

            // Act
            var result = _service.UpdateCoffePrices(coffeeName, count);

            // Assert
            Assert.IsTrue(result);
        }

    
        [Test]
        public void UpdateCoffePrices_WhenRepoReturnsFalse_ReturnsFalse()
        {
            // Arrange
            string coffeeName = "Cappuccino";
            int count = 99;

            _mockRepo
                .Setup(r => r.UpdateCoffeeStatus(coffeeName, count))
                .Returns(false);

            // Act
            var result = _service.UpdateCoffePrices(coffeeName, count);

            // Assert
            Assert.IsFalse(result);
        }

   
        [Test]
        public void UpdateCoffePrices_CallsRepositoryMethod()
        {
            // Arrange
            string coffeeName = "Mocaccino";
            int count = 3;

            _mockRepo
                .Setup(r => r.UpdateCoffeeStatus(coffeeName, count))
                .Returns(true);

            // Act
            _service.UpdateCoffePrices(coffeeName, count);

            // Assert
            _mockRepo.Verify(
                r => r.UpdateCoffeeStatus(coffeeName, count),
                Times.Once,
                "UpdateCoffeeStatus debe ser llamado exactamente 1 vez."
            );
        }
    }
}