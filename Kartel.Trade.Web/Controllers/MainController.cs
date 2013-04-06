using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kartel.Trade.Web.Controllers
{
    /// <summary>
    /// Основной контроллер системы
    /// </summary>
    public class MainController : Controller
    {
        /// <summary>
        /// Отображает главную страницу системы
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

    }
}
