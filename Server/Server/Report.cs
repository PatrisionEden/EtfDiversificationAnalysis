namespace Server
{
    public class Report
    {
        public int ReportId;
        public string Sender;
        public string ReportText;
        public string Isin;

        public Report(int reportId, string sender, string reportText, string isin)
        {
            ReportId = reportId;
            Sender = sender;
            ReportText = reportText;
            Isin = isin;
        }
    }
}
