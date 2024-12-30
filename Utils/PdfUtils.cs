using HiQPdf;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Syncfusion.HtmlConverter;
using System.Text.RegularExpressions;

namespace PdfHandlerNetCore.Utils
{
    public class PdfUtils
    {
        private const string HiQKey = "YCgJMTAE-BiwJAhIB-EhlRVE5Q-QFFAUUBQ-QFNRTlFS-TllZWVk=";
        //private const string HiQKey = "YCgJMTAE-BiwJAhIB-EhlRUE5R-V0BRQFFA-UEBTUU5R-Uk5ZWVlZ";
        public static readonly int MAX_SOURCE_SIZE = 52428800;
        public static byte[] GetPdfBytes(string html, string paperSize, bool withoutPrintStyle = false, int layout = 1, int widthPrintPDF = 890,
            bool isFitHeight = true, float marginTop = 25, float marginBottom = 10)
        {
            if (html != null && html.Length >= MAX_SOURCE_SIZE)
                throw new Exception("Dữ liệu quá lớn để tạo file pdf!");

            string[] htmlSplited = Regex.Split(html, @"<p style='page-break-before: always'/>");
            IList<byte[]> sourceFiles = new List<byte[]>();
            foreach (string _html in htmlSplited)
            {
                sourceFiles.Add(ExecuteGenPdfBytes(_html, paperSize, withoutPrintStyle, layout, widthPrintPDF, isFitHeight, marginTop, marginBottom));
            }
            byte[] fileMerged = MergeFiles(sourceFiles);
            return RemoveBlankPagesFromPdf(fileMerged);
        }

        public static byte[] ExecuteGenPdfBytes(string html, string paperSize, bool withoutPrintStyle = false, int layout = 1, int widthPrintPDF = 890,
            bool isFitHeight = true, float marginTop = 25, float marginBottom = 10)
        {
            HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
            // 1 vertical, 0 horizontal
            HtmlToPdf hiQ = new HtmlToPdf();
            PdfPageOrientation pdfPageOrientation = layout == 1 ? PdfPageOrientation.Portrait : PdfPageOrientation.Landscape;
            if (widthPrintPDF < 100) widthPrintPDF = 890;
            switch (paperSize)
            {
                case "A5":
                    hiQ.Document.PageSize = PdfPageSize.A5;
                    hiQ.Document.PageOrientation = pdfPageOrientation;
                    hiQ.BrowserWidth = widthPrintPDF;
                    hiQ.Document.Margins.Top = marginTop;
                    hiQ.Document.Margins.Bottom = marginBottom;
                    break;
                default:
                    hiQ.Document.PageSize = PdfPageSize.A4;
                    hiQ.Document.PageOrientation = pdfPageOrientation;
                    hiQ.BrowserWidth = widthPrintPDF;
                    hiQ.Document.Margins.Top = marginTop;
                    hiQ.Document.Margins.Bottom = marginBottom;
                    break;
            }

            hiQ.TriggerMode = ConversionTriggerMode.WaitTime;
            hiQ.SerialNumber = HiQKey;
            hiQ.Document.FitPageWidth = true;
            if (isFitHeight) hiQ.Document.FitPageHeight = true;
            return withoutPrintStyle ? hiQ.ConvertHtmlToMemory(html, "") : hiQ.ConvertHtmlToMemory(AddPrintStyle(html), "");
        }

        public static void GetPdfBytes(string html, string baseUrl, Stream stream)
        {
            HtmlToPdf hiQ = new HtmlToPdf();
            hiQ.SerialNumber = HiQKey;
            hiQ.Document.PageSize = PdfPageSize.A5;
            hiQ.Document.PageOrientation = PdfPageOrientation.Portrait;
            hiQ.BrowserWidth = 970;
            hiQ.Document.Margins.Top = 20;
            hiQ.Document.Margins.Bottom = 10;
            hiQ.Document.FitPageWidth = true;
            hiQ.ConvertHtmlToStream(html, baseUrl, stream);
        }

        public static void GetPdfBytesToFilePath(string html, string filePath)
        {
            HtmlToPdf hiQ = new HtmlToPdf();
            hiQ.SerialNumber = HiQKey;
            hiQ.Document.PageSize = PdfPageSize.A5;
            hiQ.Document.PageOrientation = PdfPageOrientation.Portrait;
            hiQ.BrowserWidth = 970;
            hiQ.Document.Margins.Top = 20;
            hiQ.Document.Margins.Bottom = 10;
            hiQ.Document.FitPageWidth = true;
            hiQ.ConvertHtmlToFile(html, null, filePath);
        }

        public static byte[] ConvertHtmlToMemory(string html, string baseUrl)
        {
            HtmlToPdf hiQ = new HtmlToPdf();
            hiQ.SerialNumber = HiQKey;
            hiQ.Document.PageSize = PdfPageSize.A5;
            hiQ.Document.PageOrientation = PdfPageOrientation.Landscape;
            hiQ.BrowserWidth = 970;
            hiQ.Document.Margins.Top = 20;
            hiQ.Document.Margins.Bottom = 10;
            hiQ.Document.FitPageWidth = true;
            return hiQ.ConvertHtmlToMemory(AddPrintStyle(html), baseUrl);
        }

        private static string AddPrintStyle(string html)
        {
            string pattern = @"(.VATTEMP)(\s*{)([^{}]*)(;\s)*(})";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(html);
            if (matches.Count > 0)
            {
                int i = 0;
                foreach (Match match in matches)
                {
                    if (Regex.IsMatch(match.Value, @"margin\s*:"))
                        i = -1;
                }
                if (i != -1)
                {
                    string vMatch = matches[0].Value;
                    string rs = Regex.Replace(vMatch, pattern, m => string.Format(
                                    "{0}{1}{2}{3}{4}",
                                    m.Groups[1].Value,
                                    m.Groups[2].Value,
                                    m.Groups[3].Value,
                                    "; margin: 0 auto;",
                                    m.Groups[5].Value));
                    html = html.Replace(vMatch, rs);
                }
            }
            return html;
        }

        public static string AddStyleForDisplayHtmlOnly(string html)
        {
            string pattern = @"(.VATTEMP)(\s*{)([^{}]*)(;\s)*(})";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(html);
            if (matches.Count > 0)
            {
                int i = 0;
                foreach (Match match in matches)
                {
                    if (Regex.IsMatch(match.Value, @"margin\s*:"))
                        i = -1;
                }
                if (i != -1)
                {
                    string vMatch = matches[0].Value;
                    string rs = Regex.Replace(vMatch, pattern, m => string.Format(
                                    "{0}{1}{2}{3}{4}",
                                    m.Groups[1].Value,
                                    m.Groups[2].Value,
                                    m.Groups[3].Value,
                                    "; display: block !important; margin: auto; margin-top: 50px",
                                    m.Groups[5].Value));
                    html = html.Replace(vMatch, rs);
                }
            }
            return html;
        }

        public static byte[] GetImageBytes(string html)
        {
            HtmlToImage hiQ = new HtmlToImage();
            hiQ.TriggerMode = ConversionTriggerMode.WaitTime;
            hiQ.SerialNumber = HiQKey;
            hiQ.BrowserWidth = 970;
            hiQ.TransparentImage = false;
            return hiQ.ConvertHtmlToMemory(html, "");
        }
        public static byte[] MergeFiles(IList<byte[]> sourceFiles)
        {

            using (MemoryStream ms = new MemoryStream())
            using (iText.Kernel.Pdf.PdfDocument pdf = new iText.Kernel.Pdf.PdfDocument(new PdfWriter(ms).SetSmartMode(true)))
            {
                foreach (var sourceFile in sourceFiles)
                {
                    using (MemoryStream memoryStream = new MemoryStream(sourceFile))
                    {
                        // Create reader from bytes
                        using (PdfReader reader = new PdfReader(memoryStream))
                        {
                            iText.Kernel.Pdf.PdfDocument srcDoc = new iText.Kernel.Pdf.PdfDocument(reader);
                            srcDoc.CopyPagesTo(1, srcDoc.GetNumberOfPages(), pdf);
                        }
                    }
                }

                // Close pdf
                pdf.Close();

                // Return array
                return ms.ToArray();
            }
        }


        public static byte[] RemoveBlankPagesFromPdf(byte[] inputPdfBytes)
        {
            using (MemoryStream inputMemoryStream = new MemoryStream(inputPdfBytes))
            using (MemoryStream outputMemoryStream = new MemoryStream())
            {
                using (PdfReader reader = new PdfReader(inputMemoryStream))
                using (PdfWriter writer = new PdfWriter(outputMemoryStream))
                {
                    using (iText.Kernel.Pdf.PdfDocument pdfDocument = new iText.Kernel.Pdf.PdfDocument(reader, writer))
                    {
                        int pageCount = pdfDocument.GetNumberOfPages();
                        int blankPageCount = 0;

                        for (int i = pageCount; i >= 1; i--)
                        {
                            iText.Kernel.Pdf.PdfPage page = pdfDocument.GetPage(i);

                            // Check if the page is blank
                            if (IsPageBlank(page))
                            {
                                pdfDocument.RemovePage(i);
                                blankPageCount++;
                            }
                        }

                        Console.WriteLine($"Removed {blankPageCount} blank pages.");
                    }
                }

                return outputMemoryStream.ToArray();
            }
        }

        private static bool IsPageBlank(iText.Kernel.Pdf.PdfPage page)
        {
            PdfDictionary pageDictionary = page.GetPdfObject();
            iText.Kernel.Pdf.PdfObject contents = pageDictionary.Get(PdfName.Contents);

            // A page is considered blank if it has no contents or if the contents are empty
            if (contents != null && contents.ToString().Trim().Length != 0)
            {
                string extractedText = PdfTextExtractor.GetTextFromPage(page);
                return string.IsNullOrEmpty(extractedText);
            }
            return true;
        }


        public static byte[] ConvertPdfWithBlink(HtmlToPdfConverter htmlConverter, string htmlText)
        {
            string baseUrl = @"";
            var document = htmlConverter.Convert(htmlText, baseUrl);
            MemoryStream pdfConvertedStream = new MemoryStream();
            document.Save(pdfConvertedStream);
            byte[] pdfByte = pdfConvertedStream.ToArray();
            pdfConvertedStream.Close();
            pdfConvertedStream.Dispose();
            return pdfByte;
        }
    }
}