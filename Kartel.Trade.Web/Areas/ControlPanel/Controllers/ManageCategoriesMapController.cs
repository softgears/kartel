using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kartel.Trade.Web.Areas.ControlPanel.Controllers
{
    public class ManageCategoriesMapController : BaseRootController
    {
        //
        // GET: /ControlPanel/ManageCategoriesMap/

        public ActionResult Index()
        {
            return ControlPanelSectionView("Управления картой категорий", "categoriesMap", "CategoriesMap.js");
        }

    }
}
