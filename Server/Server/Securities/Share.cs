namespace Server
{
    public class Share : Security
    {
        public string Country;
        public string Sector;
        public string Industry;
        public Share(string isin, string ticker, string name, double price,
            string country, string sector, string industry)
            : base(isin, ticker, name, price)
        {
            Country = country;
            Sector = sector;
            Industry = industry;
        }
    }
}
