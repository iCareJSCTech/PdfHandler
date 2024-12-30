using System.ComponentModel.DataAnnotations;

namespace PdfHandlerNetCore.Models
{
    public class HtmlToPdfRequest
    {
        [Required]
        public byte[] HtmlBytes { get; set; }
        public string? PaperSize { get; set; }
        public bool WithoutPrintStyle { get; set; } = true;
        public int Layout { get; set; } = 1;
        public int WidthPrintPDF { get; set; } = 890;
        public string? FileName { get; set; }
        public bool? IsFitHeight { get; set; }
        public float Top { get; set; } = 25;
        public float Bottom { get; set; } = 10;
    }
}