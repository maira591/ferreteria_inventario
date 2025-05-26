using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class LogController : Controller
    {

        #region Private Fields
        private readonly IHttpContextAccessor _accessor;
        #endregion

        #region Constructor
        public LogController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        #endregion

        #region Public methods

        #endregion
        public ActionResult Index()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            ViewBag.TotalRows = 0;
            return View();
        }

        [HttpGet]
        public void SetDates(string startDate, string endDate)
        {
            HttpContext.Session.SetString("LogStartDate", startDate);
            HttpContext.Session.SetString("LogEndDate", endDate);
        }

        [HttpGet]
        public PartialViewResult IndexGrid()
        {
            var startDate = _accessor.HttpContext.Session.GetString("LogStartDate");
            var endDate = _accessor.HttpContext.Session.GetString("LogEndDate");

            if (startDate == null)
            {
                return PartialView("_IndexGrid", new List<string>());
            }

            string serverPath = Environment.CurrentDirectory + "\\logs";

#if DEBUG
            serverPath = Environment.CurrentDirectory + "\\bin\\Debug\\net5.0\\logs";
#endif

            var logs = Directory.GetFiles(serverPath, "*.log").
                Select(x => new LogModel
                {
                    Archivo = x.Replace(serverPath, "").Replace("\\", "").Replace(".log", ""),
                    Fecha = Utilities.DateParse(x.Replace(serverPath, "").Replace("\\", "").Replace(".log", "").Split('_')[0]),
                    Nombre = x.Replace(serverPath, "").Replace("\\", "").Replace(".log", "").Split('_')[1]
                }).Where(x => x.Fecha >= Utilities.DateParse(startDate) &&
                    x.Fecha <= Utilities.DateParse(endDate))
                .OrderByDescending(s => s.Archivo).ToList();

            ViewBag.TotalRows = logs.Count;
            return PartialView("_IndexGrid", logs);
        }

        public ActionResult Download(string log)
        {
            string path = $"{Environment.CurrentDirectory}\\logs\\{log}.log";
#if DEBUG
            path = $"{Environment.CurrentDirectory}\\bin\\Debug\\net5.0\\logs\\{log}.log";
#endif

            if (System.IO.File.Exists(path))
            {
                return PhysicalFile(path, "Application/log", $"{log}.log");
                //return PhysicalFile(path, "Application/log", $"{log}.log");
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}
