﻿using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace NetCoreWebApiPrintPDF.Infrastructure.Shared.Services
{
    public class HtmlToPdfService : IHtmlToPdfService
    {
        private readonly IBrowser _browser;
        private readonly PdfOptions _pdfOptions;

        public HtmlToPdfService()
        {
            var browserFetcher = new BrowserFetcher();
            browserFetcher.DownloadAsync().GetAwaiter().GetResult();

            _browser = Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            }).GetAwaiter().GetResult();

            _pdfOptions = new PdfOptions
            {
                Format = PaperFormat.A4,  // Keep the format as A4
                Landscape = true,         // Set landscape mode to true
                MarginOptions = new MarginOptions
                {
                    Top = "10mm",
                    Right = "10mm",
                    Bottom = "10mm",
                    Left = "10mm",
                }
            };
        }

        public async Task<byte[]> ToByteArray(string htmlContent)
        {
            using var page = await _browser.NewPageAsync();

            await page.SetContentAsync(htmlContent);

            var data = await page.PdfDataAsync(_pdfOptions);

            await page.CloseAsync();

            return data;
        }
    }
}