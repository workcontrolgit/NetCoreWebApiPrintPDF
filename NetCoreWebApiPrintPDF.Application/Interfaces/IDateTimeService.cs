using System;

namespace NetCoreWebApiPrintPDF.Application.Interfaces
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
    }
}