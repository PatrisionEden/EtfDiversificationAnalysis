namespace Server
{
    public class Bond : Security
    {
        public Bond(string isin, string name) : base(isin, "*Bond", name, 0)
        {
        }
    }
}
