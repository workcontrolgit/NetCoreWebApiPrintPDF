using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using NetCoreWebApiPrintPDF.WebApi.Models;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using System.IO;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace NetCoreWebApiPrintPDF.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CertificateController : BaseApiController
    {
        private readonly IServiceProvider _serviceProvider;

        public CertificateController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpPost("GenerateCertificate")]
        public async Task<IActionResult> GenerateCertificate([FromBody] CertificateViewModel model)
        {
            var htmlContent = await RenderRazorViewToString(model);
            var pdfBytes = GeneratePdfFromHtml(htmlContent);

            return File(pdfBytes, "application/pdf", $"{model.StudentName}_certificate.pdf");
        }

        [NonAction]
        public byte[] GeneratePdfFromHtml(string htmlContent)
        {
            var document = new PdfDocument();

            PdfGenerator.AddPdfPages(document, htmlContent, PageSize.A4);

            //var pdfDocument = PdfGenerator.GeneratePdf(htmlContent, PdfSharp.PageSize.A4, 20);

            using (var stream = new MemoryStream())
            {
                document.Save(stream);
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