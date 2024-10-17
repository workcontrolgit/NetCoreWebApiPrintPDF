namespace NetCoreWebApiPrintPDF.Application.Interfaces
{
    public interface IHtmlToPdfService
    {
        Task<byte[]> ToByteArray(string htmlContent);

        //Task ToFile(string htmlContent, string outputFilePath);
    }
}