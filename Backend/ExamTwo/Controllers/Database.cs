namespace ExamTwo.Controllers
{
    public class Database
    {

        private Dictionary<string, int> keyValues = new Dictionary<string, int>
        {
            { "Americano", 1 },
            { "Cappuccino", 8 },
            { "Lates", 10 },
            { "Mocaccino", 15}
        };

        private Dictionary<string, int> keyValues2 = new Dictionary<string, int>
        {
            { "Americano", 950 },
            { "Cappuccino", 1200 },
            { "Lates", 1350 },
            { "Mocaccino", 1500}
        };

        private Dictionary<int, int> keyValues3 = new Dictionary<int, int>
        {
            { 500, 20 },
            { 100, 30 },
            { 50, 50 },
            { 25, 25}
        };

        public Dictionary<string, int> getAllCoffeeData()
        {
            return this.keyValues;
        }

        public Dictionary<string, int> getCofeePrices()
        {
            return this.keyValues2;
        }

        public Dictionary<int, int> getChangeQuantity()
        {
            return this.keyValues3;
        } 

        public bool UpdateCoffeeStatus(string coffeeName, int count)
        {
            int actualCount = this.keyValues[coffeeName];

            if(actualCount>= count)
            {
                this.keyValues[coffeeName] = actualCount - count;
                return true;
            }

            return false;
        }

        public bool UpdateCoinStatus(int coins, int count)
        {
            this.keyValues3[coins] += count;

            return true;
        }

    }
}
