namespace Server
{
    public class InitialData
    {
        public UserData UserData;
        public List<KeyValuePair<string, double>> IsinToPrice;
        public List<KeyValuePair<string, string>> IsinToTicker;
        public InitialData(UserData userData,
            List<KeyValuePair<string, double>> isinToPrice, List<KeyValuePair<string, string>> isinToTicker)
        {
            UserData = userData;
            IsinToPrice = isinToPrice;
            IsinToTicker = isinToTicker;
        }
    }
    //public class KeyValuePair<TKey, TValue> {
    //    public TKey Key;
    //    public TValue Value;

    //    public KeyValuePair(TKey key, TValue value)
    //    {
    //        Key = key;
    //        Value = value;
    //    }
    //}
}
