namespace Server
{
    public abstract class Security
    {
        public readonly string Isin;
        public readonly string Ticker;
        public readonly string Name;
        public readonly double Price;

        protected Security(string isin, string ticker, string name, double price)
        {
            Isin = isin;
            Ticker = ticker;
            Name = name;
            Price = price;
        }
    }
}
