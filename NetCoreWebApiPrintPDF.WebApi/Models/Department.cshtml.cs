using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NetCoreWebApiPrintPDF.WebApi.Models;

public class DepartmentListModel : PageModel
{
    public IEnumerable<GetDepartmentsViewModel> Departments { get; set; }

    // OnGet method with parameter binding
    public void OnGet(List<GetDepartmentsViewModel> departments)
    {
        // Assign the passed list to the property
        Departments = departments;
    }
}