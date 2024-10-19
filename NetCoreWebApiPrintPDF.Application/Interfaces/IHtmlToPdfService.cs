namespace NetCoreWebApiPrintPDF.Application.Interfaces
{
    public interface IHtmlToPdfService
    {
        Task<byte[]> ToByteArray(string htmlContent);
    }
}