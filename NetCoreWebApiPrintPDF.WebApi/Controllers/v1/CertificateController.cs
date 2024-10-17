using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using NetCoreWebApiPrintPDF.Application.Helpers;
using NetCoreWebApiPrintPDF.Application.Interfaces;
using NetCoreWebApiPrintPDF.Domain.Entities;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using System.IO;
using System.Threading;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace NetCoreWebApiPrintPDF.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CertificateController : BaseApiController
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly RazorViewToStringRenderer _razorViewToStringRenderer;

        public CertificateController(IServiceProvider serviceProvider, IHtmlToPdfService htmlToPdfService, RazorViewToStringRenderer razorViewToStringRenderer)
        {
            _serviceProvider = serviceProvider;
            _htmlToPdfService = htmlToPdfService;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        [HttpPost("GenerateCertificate")]
        public async Task<IActionResult> GenerateCertificate([FromBody] Certificate model, CancellationToken cancellationToken)
        {
            //var htmlContent = await RenderRazorViewToString(model);
            var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("~/Views/CertificateTemplate.cshtml", model);
            var pdfBytes = GeneratePdfFromHtml(htmlContent);

            return File(pdfBytes, "application/pdf", $"{model.StudentName}_certificate.pdf");
        }

        [HttpPost("PrintPdf")]
        public async Task<FileResult> PrintPdf([FromBody] Certificate model, CancellationToken cancellationToken)
        {
            // var htmlContent = await RenderRazorViewToString(model);
            var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("~/Views/CertificateTemplate.cshtml", model);
            var pdfContent = await _htmlToPdfService.ToByteArray(htmlContent);
            return File(pdfContent, "application/pdf", $"{model.StudentName}_certificate.pdf");
        }

        [NonAction]
        public byte[] GeneratePdfFromHtml(string htmlContent)
        {
            var document = new PdfDocument();

            PdfGenerator.AddPdfPages(document, htmlContent, PageSize.A4);

            using (var stream = new MemoryStream())
            {
                document.Save(stream, false);
                return stream.ToArray();
            }
        }

    }
}