﻿namespace NetCoreWebApiPrintPDF.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class DepartmentsController : BaseApiController
    {
        /// <summary>
        /// Gets a list of employees based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter used to get the list of employees.</param>
        /// <returns>A list of employees.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetDepartmentsQuery filter)
        {
            return Ok(await Mediator.Send(filter));
        }

        /// <summary>
        /// Print a list of employees based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter used to get the list of employees.</param>
        /// <returns>A list of employees.</returns>
        [HttpGet]
        [Route("Print")]
        public async Task<FileResult> Export([FromQuery] GetDepartmentsPrint filter)
        {
            var result = await Mediator.Send(filter);
            return File(result, "application/pdf", "DepartmentList.pdf");
        }
    }
}