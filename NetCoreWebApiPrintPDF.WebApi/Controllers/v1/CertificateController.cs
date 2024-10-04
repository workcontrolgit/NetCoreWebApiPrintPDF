using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using NetCoreWebApiPrintPDF.Infrastructure.Shared.Services;
using NetCoreWebApiPrintPDF.WebApi.Models;
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
        private readonly HtmlToPdfService _htmlToPdfService;

        public CertificateController(IServiceProvider serviceProvider, HtmlToPdfService htmlToPdfService)
        {
            _serviceProvider = serviceProvider;
            _htmlToPdfService = htmlToPdfService;
        }

        private string htmlContent2 = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Invoice</title>
    <!-- Bootstrap CSS -->
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"" rel=""stylesheet"">
    <style>
        .invoice-box {
            max-width: 800px;
            margin: 20px auto;
            padding: 30px;
            border: 1px solid #eee;
            background-color: #fff;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.15);
        }
        .invoice-title {
            font-size: 45px;
            color: #333;
        }
        .invoice-heading {
            background-color: #f8f9fa;
            font-weight: bold;
        }
        .total {
            font-weight: bold;
            border-top: 2px solid #eee;
        }
    </style>
</head>
<body>
    <div class=""invoice-box"">
        <div class=""row mb-4"">
            <div class=""col-md-6"">
                <h2 class=""invoice-title"">Invoice</h2>
            </div>
            <div class=""col-md-6 text-end"">
                Invoice #: 123<br>
                Created: January 1, 2024<br>
                Due: January 15, 2024
            </div>
        </div>

        <div class=""row mb-4"">
            <div class=""col-md-6"">
                <strong>Billing to:</strong><br>
                John Doe<br>
                1234 Main St.<br>
                Springfield, IL 62704
            </div>
            <div class=""col-md-6 text-end"">
                <strong>Company Name</strong><br>
                info@company.com
            </div>
        </div>

        <table class=""table table-bordered table-striped"">
            <thead>
                <tr>
                    <td>Item</td>
                    <td class=""text-end"">Price</td>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>Website design</td>
                    <td class=""text-end"">$300.00</td>
                </tr>
                <tr>
                    <td>Hosting (3 months)</td>
                    <td class=""text-end"">$75.00</td>
                </tr>
                <tr>
                    <td>Domain name (1 year)</td>
                    <td class=""text-end"">$10.00</td>
                </tr>
                <tr>
                    <td>SEO Optimization</td>
                    <td class=""text-end"">$150.00</td>
                </tr>
            </tbody>
            <tfoot>
                <tr class=""fw-bold"">
                    <td></td>
                    <td class=""text-end fw-bold"">Total: $535.00</td>
                </tr>
            </tfoot>
        </table>
    </div>

    <!-- Bootstrap JS (Optional if you need Bootstrap's JavaScript components) -->
    <script src=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js""></script>
</body>
</html>

    ";

        [HttpPost("GenerateCertificate")]
        public async Task<IActionResult> GenerateCertificate([FromBody] CertificateViewModel model, CancellationToken cancellationToken)
        {
            var htmlContent = await RenderRazorViewToString(model);
            var pdfBytes = GeneratePdfFromHtml(htmlContent2);

            return File(pdfBytes, "application/pdf", $"{model.StudentName}_certificate.pdf");
        }

        [HttpPost("PrintPdf")]
        public async Task<FileResult> PrintPdf([FromBody] CertificateViewModel model, CancellationToken cancellationToken)
        {
            var htmlContent = await RenderRazorViewToString(model);
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

        [NonAction]
        public async Task<string> RenderRazorViewToString(CertificateViewModel model)
        {
            var viewEngine = _serviceProvider.GetRequiredService<IRazorViewEngine>();
            var tempDataProvider = _serviceProvider.GetRequiredService<ITempDataProvider>();
            var actionContext = new ActionContext(
                new DefaultHttpContext { RequestServices = _serviceProvider },
                new RouteData(),
                new ActionDescriptor()
            );

            using (var sw = new StringWriter())
            {
                //var viewResult = viewEngine.FindView(actionContext, "~/Views/CertificateTemplate", false);

                var viewResult = viewEngine.GetView("~/Views/CertificateTemplate.cshtml", "~/Views/CertificateTemplate.cshtml", false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"View 'Views/CertificateTemplate' not found.");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}