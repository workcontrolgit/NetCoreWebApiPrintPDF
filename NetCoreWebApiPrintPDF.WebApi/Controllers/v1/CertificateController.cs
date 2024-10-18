using NetCoreWebApiPrintPDF.Application.Interfaces;
using NetCoreWebApiPrintPDF.Domain.Entities;
using System.Threading;

namespace NetCoreWebApiPrintPDF.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CertificateController : BaseApiController
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHtmlToPdfService _htmlToPdfService;
        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        public CertificateController(IServiceProvider serviceProvider, IHtmlToPdfService htmlToPdfService, IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            _serviceProvider = serviceProvider;
            _htmlToPdfService = htmlToPdfService;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        [HttpPost("PrintPdf")]
        public async Task<FileResult> PrintPdf([FromBody] Certificate model, CancellationToken cancellationToken)
        {
            var sampleModel = CreateSampleCertificate();

            var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("~/Views/CertificateTemplate.cshtml", sampleModel);
            var pdfContent = await _htmlToPdfService.ToByteArray(htmlContent);
            return File(pdfContent, "application/pdf", $"{sampleModel.StudentName}_certificate.pdf");
        }

        private static Certificate CreateSampleCertificate()
        {
            List<SkillGrade> skillGrades = CreateSampleSkillGrades();

            // Initialize the Certificate
            var certificate = new Certificate
            {
                StudentName = "John Doe",
                CoachName = "Fuji Nguyen",
                EvaluationDate = DateTime.Now,
                SkillGrades = skillGrades
            };
            // Return the certificate
            return certificate;
        }

        private static List<SkillGrade> CreateSampleSkillGrades()
        {
            // Create a list of SkillGrades (you need to define SkillGrade class or use existing one)
            return new List<SkillGrade>
        {
            new SkillGrade { SkillName = "Forehand", Grade = "A" },
            new SkillGrade { SkillName = "Backhand", Grade = "B" }
        };
        }
    }
}