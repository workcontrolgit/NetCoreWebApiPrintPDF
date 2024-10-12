namespace NetCoreWebApiPrintPDF.Application.Features.Employees.Queries.ExportEmployees
{
    /// <summary>
    /// ExportAllEmployeesQuery - handles media IRequest
    /// BaseRequestParameter - contains paging parameters
    /// To add filter/search parameters, add search properties to the body of this class
    /// </summary>
    public class ExportEmployeesQuery : QueryParameter, IRequest<PagedResponse<IEnumerable<Entity>>>
    {
        //examples:
        public string LastName { get; set; }

        public string FirstName { get; set; }
        public string Email { get; set; }
        public string EmployeeNumber { get; set; }
        public string PositionTitle { get; set; }

        public ListParameter ShapeParameter { get; set; }
    }

    public class ExportAllEmployeesQueryHandler : IRequestHandler<ExportEmployeesQuery, PagedResponse<IEnumerable<Entity>>>
    {
        private readonly IEmployeeRepositoryAsync _employeeRepository;
        private readonly IModelHelper _modelHelper;

        /// <summary>
        /// Constructor for ExportAllEmployeesQueryHandler class.
        /// </summary>
        /// <param name="employeeRepository">IEmployeeRepositoryAsync object.</param>
        /// <param name="modelHelper">IModelHelper object.</param>
        /// <returns>
        /// ExportAllEmployeesQueryHandler object.
        /// </returns>
        public ExportAllEmployeesQueryHandler(IEmployeeRepositoryAsync employeeRepository, IModelHelper modelHelper)
        {
            _employeeRepository = employeeRepository;
            _modelHelper = modelHelper;
        }

        /// <summary>
        /// Handles the ExportEmployeesQuery request and returns a PagedResponse containing the requested data.
        /// </summary>
        /// <param name="request">The ExportEmployeesQuery request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A PagedResponse containing the requested data.</returns>
        public async Task<PagedResponse<IEnumerable<Entity>>> Handle(ExportEmployeesQuery request, CancellationToken cancellationToken)
        {
            var objRequest = request;
            //filtered fields security
            if (!string.IsNullOrEmpty(objRequest.Fields))
            {
                //limit to fields in view model
                objRequest.Fields = _modelHelper.ValidateModelFields<ExportEmployeesViewModel>(objRequest.Fields);
            }
            else
            {
                //default fields from view model
                objRequest.Fields = _modelHelper.GetModelFields<ExportEmployeesViewModel>();
            }
            // query based on filter
            var qryResult = await _employeeRepository.ExportEmployeeResponseAsync(objRequest);
            var data = qryResult.data;
            RecordsCount recordCount = qryResult.recordsCount;

            // response wrapper
            return new PagedResponse<IEnumerable<Entity>>(data, objRequest.PageNumber, objRequest.PageSize, recordCount);
        }
    }
}