namespace NetCoreWebApiPrintPDF.Application.Features.Departments.Queries.GetDepartments
{
    /// <summary>
    /// GetAllDepartmentsPrint - handles media IRequest
    /// BaseRequestParameter - contains paging parameters
    /// To add filter/search parameters, add search properties to the body of this class
    /// </summary>
    public class GetDepartmentsPrint : ListParameter, IRequest<byte[]>
    {
    }

    public class GetAllDepartmentsPrintHandler : IRequestHandler<GetDepartmentsPrint, byte[]>
    {
        private readonly IDepartmentRepositoryAsync _repository;
        private readonly IModelHelper _modelHelper;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHtmlToPdfService _htmlToPdfService;

        private readonly IRazorViewToStringRenderer _razorViewToStringRenderer;

        /// <summary>
        /// Constructor for GetAllDepartmentsPrintHandler class.
        /// </summary>
        /// <param name="repository">IDepartmentRepositoryAsync object.</param>
        /// <param name="modelHelper">IModelHelper object.</param>
        /// <returns>
        /// GetAllDepartmentsPrintHandler object.
        /// </returns>
        public GetAllDepartmentsPrintHandler(IDepartmentRepositoryAsync repository, IModelHelper modelHelper, IMapper mapper, IServiceProvider serviceProvider, IHtmlToPdfService htmlToPdfService, IRazorViewToStringRenderer razorViewToStringRenderer)
        {
            _repository = repository;
            _modelHelper = modelHelper;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _htmlToPdfService = htmlToPdfService;
            _razorViewToStringRenderer = razorViewToStringRenderer;
        }

        /// <summary>
        /// Handles the GetDepartmentsQuery request and returns a PagedResponse containing the requested data.
        /// </summary>
        /// <param name="request">The GetDepartmentsQuery request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A PagedResponse containing the requested data.</returns>
        public async Task<byte[]> Handle(GetDepartmentsPrint request, CancellationToken cancellationToken)
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

            var htmlContent = await _razorViewToStringRenderer.RenderViewToStringAsync("~/Views/Department.cshtml", viewModel);

            var pdfContent = await _htmlToPdfService.ToByteArray(htmlContent);

            return pdfContent;
        }
    }
}