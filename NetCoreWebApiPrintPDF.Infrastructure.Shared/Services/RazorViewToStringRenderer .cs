using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;

namespace NetCoreWebApiPrintPDF.Infrastructure.Shared.Services
{
    public class RazorViewToStringRenderer : IRazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorViewToStringRenderer(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            var actionContext = new ActionContext(HttpContext, RouteData, new ActionDescriptor());

            var view = _viewEngine.FindView(actionContext, viewName, false).View;

            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
                    new TempDataDictionary(HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await view.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}