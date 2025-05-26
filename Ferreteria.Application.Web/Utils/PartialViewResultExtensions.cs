using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;

namespace Ferreteria.Application.Website.Utils
{
    public static class PartialViewResultExtensions
    {
        public static string ConvertToString(this PartialViewResult partialView, ControllerContext controllerContext)
        {
            using (var sw = new StringWriter())
            {
                var vista = partialView.ViewEngine.FindView(controllerContext, partialView.ViewName, false).View;

                var vc = new ViewContext(controllerContext, vista, partialView.ViewData, partialView.TempData, sw, null);
                vista.RenderAsync(vc);

                var partialViewString = sw.GetStringBuilder().ToString(); // vista.ToString();

                return partialViewString;
            }
        }
    }
}