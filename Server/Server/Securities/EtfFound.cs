namespace Server
{
    public class EtfFound : Security
    {
        public EtfFound(string isin, string ticker, double price) : base(isin, ticker, ticker, price)
        {
        }
    }
}
