using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System.IO;

namespace NetCoreWebApiPrintPDF.Application.Features.Departments.Queries.GetDepartments
{
    /// <summary>
    /// GetAllDepartmentsQuery - handles media IRequest
    /// BaseRequestParameter - contains paging parameters
    /// To add filter/search parameters, add search properties to the body of this class
    /// </summary>
    public class GetDepartmentsQuery : ListParameter, IRequest<byte[]>
    {
    }

    public class GetAllDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, byte[]>
    {
        private readonly IDepartmentRepositoryAsync _repository;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHtmlToPdfService _htmlToPdfService;

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

        /// <summary>
        /// Constructor for GetAllDepartmentsQueryHandler class.
        /// </summary>
        /// <param name="repository">IDepartmentRepositoryAsync object.</param>
        /// <param name="modelHelper">IModelHelper object.</param>
        /// <returns>
        /// GetAllDepartmentsQueryHandler object.
        /// </returns>
        public GetAllDepartmentsQueryHandler(IDepartmentRepositoryAsync repository, IModelHelper modelHelper, IMapper mapper, IServiceProvider serviceProvider, IHtmlToPdfService htmlToPdfService)
        {
            _repository = repository;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _htmlToPdfService = htmlToPdfService;
        }

        /// <summary>
        /// Handles the GetDepartmentsQuery request and returns a PagedResponse containing the requested data.
        /// </summary>
        /// <param name="request">The GetDepartmentsQuery request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A PagedResponse containing the requested data.</returns>
        public async Task<byte[]> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            string fields = _modelHelper.GetModelFields<GetDepartmentsViewModel>();
            string defaultOrderByColumn = "Name";

            string orderBy = string.Empty;

            // if the request orderby is not null
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                // check to make sure order by field is valid and in the view model
                orderBy = _modelHelper.ValidateModelFields<GetDepartmentsViewModel>(request.OrderBy);
            }

            // if the order by is invalid
            if (string.IsNullOrEmpty(orderBy))
            {
                //default fields from view model
                orderBy = defaultOrderByColumn;
            }

            var data = await _repository.GetAllShapeAsync(orderBy, fields);

            // automap to ViewModel
            var viewModel = _mapper.Map<IEnumerable<GetDepartmentsViewModel>>(data);
            // Create a list of SkillGrades (you need to define SkillGrade class or use existing one)
            List<SkillGrade> skillGrades = new List<SkillGrade>
        {
            new SkillGrade { SkillName = "Forehand", Grade = "A" },
            new SkillGrade { SkillName = "Backhand", Grade = "B" }
        };

            // Initialize the CertificateViewModel
            var certificate = new Certificate
            {
                StudentName = "John Doe",
                CoachName = "Fuji Nguyen",
                EvaluationDate = DateTime.Now,
                SkillGrades = skillGrades
            };

            //var htmlContent = await RenderRazorViewToString(viewModel);
            var htmlContent = await RenderRazorViewToString(certificate);
            var pdfContent = await _htmlToPdfService.ToByteArray(htmlContent);

            //return viewModel;
            return pdfContent;
        }

        public async Task<string> RenderRazorViewToString(Certificate model)
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
                var viewResult = viewEngine.GetView("~/Views/CertificateTemplate.cshtml", "~/Views/CertificateTemplate.cshtml", true);

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