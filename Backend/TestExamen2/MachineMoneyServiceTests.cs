using NUnit.Framework;
using Moq;
using ExamTwo.Interfaces;
using ExamTwo.Services;
using System.Collections.Generic;

namespace TestExamen2
{
    [TestFixture]
    public class MachineMoneyServiceTests
    {
        private Mock<ICoffeMachineRepository> _mockRepo;
        private MachineMoneyService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<ICoffeMachineRepository>();
            _service = new MachineMoneyService(_mockRepo.Object);
        }

        [Test]
        public void GetCoinInventory_ReturnsRepositoryValues()
        {
            var expected = new Dictionary<int, int>
            {
                { 500, 10 },
                { 100, 20 },
                { 50, 50 }
            };

            _mockRepo.Setup(r => r.GetCoinInventory()).Returns(expected);

            var result = _service.GetCoinInventory();

            Assert.AreEqual(expected, result);
        }

      
        [Test]
        public void UpdateCoins_WhenRepoReturnsTrue_ReturnsTrue()
        {
        
            int coin = 100;
            int count = 5;

            _mockRepo
                .Setup(r => r.UpdateCoinStauts(coin, count))
                .Returns(true);

            
            var result = _service.UpdateCoins(coin, count);

            // Assert
            Assert.IsTrue(result);
        }

     
        [Test]
        public void UpdateCoins_WhenRepoReturnsFalse_ReturnsFalse()
        {
            // Arrange
            int coin = 500;
            int count = -10;

            _mockRepo
                .Setup(r => r.UpdateCoinStauts(coin, count))
                .Returns(false);

            // Act
            var result = _service.UpdateCoins(coin, count);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void UpdateCoins_CallsRepositoryMethod()
        {
            // Arrange
            int coin = 25;
            int count = 3;

            _mockRepo
                .Setup(r => r.UpdateCoinStauts(coin, count))
                .Returns(true);

            // Act
            _service.UpdateCoins(coin, count);

            // Assert
            _mockRepo.Verify(
                r => r.UpdateCoinStauts(coin, count),
                Times.Once,
                "UpdateCoinStauts debe ser llamado exactamente 1 vez."
            );
        }
    }
}
