using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class FinexEtfHoldingsFileParser
    {
        public IEnumerable<(string Isin, string ShareName, double PartInEtf)> ParseHoldingsFile(string path)
        {
            var txt = ExtractTextFromPDF(path);

            string[] lines = txt.Split("\n")
                .Where(s => s.Length != 0 && Char.IsDigit(s[0]) && s[s.Length - 1] == '%')
                .ToArray();

            foreach (var row in lines)
            {
                string[] cells = row.Split(" ");

                string isin = cells[cells.Length - 3];
                string shareName = row.Substring(2, row.IndexOf(isin) - 3);
                string doubleInString = cells[cells.Length - 2];
                double partInEtf = double.Parse(doubleInString, CultureInfo.InvariantCulture);

                yield return (isin, shareName, partInEtf);
            }
        }
        public string ExtractTextFromPDF(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            Stream stream = fileInfo.OpenRead();
            stream.Position = 0;
            PdfReader pdfReader = new PdfReader(stream);
            PdfDocument pdfDoc = new PdfDocument(pdfReader);
            StringBuilder pageContent = new StringBuilder();
            for (int page = 1; page <= pdfDoc.GetNumberOfPages(); page++)
            {
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                pageContent.Append(PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy));
                pageContent.Append('\n');
            }
            pdfDoc.Close();
            pdfReader.Close();
            return pageContent.ToString();
        }
    }
}
