namespace NetCoreWebApiPrintPDF.Infrastructure.Persistence.Repositories
{
    public class DepartmentRepositoryAsync : GenericRepositoryAsync<Department>, IDepartmentRepositoryAsync
    {
        public DepartmentRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}