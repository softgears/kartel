using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;

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

        /// <summary>
        /// Отображает указанную категорию
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns></returns>
        public ActionResult Category(long id)
        {
            // Загружаем указанную категорию
            var rep = Locator.GetService<ICategoriesRepository>();
            var cat = rep.Load(id);
            if (cat == null)
            {
                return RedirectToAction("NotFound");
            }

            // Отображаем вьху указанной категории
            return View(cat);
        }

    }
}
