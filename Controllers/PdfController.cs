using DotNet.RateLimiter.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using PdfHandlerNetCore.Models;
using PdfHandlerNetCore.Utils;
using Syncfusion.HtmlConverter;
using System.Diagnostics;
using System.Text;

namespace PdfHandlerNetCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly ILogger<PdfController> _logger;
        private string _rootPath;

        public PdfController(ILogger<PdfController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _rootPath = hostingEnvironment.ContentRootPath;
        }

        [HttpPost]
        [Route("html-to-pdf")]
        [RateLimit(PeriodInSec = 1, Limit = 6)]
        public IActionResult HtmlToPdf(HtmlToPdfRequest request)
        {
            string html = Encoding.UTF8.GetString(request.HtmlBytes);
            try
            {
                var watch = Stopwatch.StartNew();
                watch.Start();
                var pdfFile = PdfUtils.GetPdfBytes(html, request.PaperSize, request.WithoutPrintStyle, request.Layout, request.WidthPrintPDF,
                    request.IsFitHeight ?? true, request.Top, request.Bottom);
                watch.Stop();
                _logger.LogInformation("Time convert: " + watch.Elapsed.TotalSeconds +
                    $@", source length: {request.HtmlBytes.Length}, domain: {(Request.Headers.TryGetValue("Client-Domain", out var domain) ? domain.FirstOrDefault() : "")}");
                return File(pdfFile, "application/pdf", request.FileName ?? "converted-file.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        [HttpPost]
        [Route("html-to-pdf-blink")]
        [RateLimit(PeriodInSec = 1, Limit = 6)]
        public IActionResult HtmlToPdfBlink(HtmlToPdfRequest request)
        {
            string html = Encoding.UTF8.GetString(request.HtmlBytes);
            try
            {
                var watch = Stopwatch.StartNew();
                watch.Start();
                HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
                BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
                blinkConverterSettings.MediaType = MediaType.Print;
                blinkConverterSettings.PdfPageSize = Syncfusion.Pdf.PdfPageSize.A4;
                blinkConverterSettings.Scale = 1;
                htmlConverter.ConverterSettings = blinkConverterSettings;
                var pdfFile = PdfUtils.ConvertPdfWithBlink(htmlConverter, html);
                string folder = Path.Combine(_rootPath, "PdfFiles");
                Directory.CreateDirectory(folder)
;
                watch.Stop();
                _logger.LogInformation("Time convert: " + watch.Elapsed.TotalSeconds +
                    $@", source length: {request.HtmlBytes.Length}, domain: {(Request.Headers.TryGetValue("Client-Domain", out var domain) ? domain.FirstOrDefault() : "")}");
                return File(pdfFile, "application/pdf", request.FileName ?? "converted-file.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}