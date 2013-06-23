using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.HtmlControls;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Exceptions;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Areas.ControlPanel.Classes;
using Kartel.Trade.Web.Areas.ControlPanel.Models;

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

        [HttpPost]
        [AccessAuthorize]
        public JsonResult GetCategoryMaps()
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<ICategoriesMapRepository>();

                // Список
                var items = repository.FindAll().OrderByDescending(n => n.SortOrder).Select(n => new CategoryMapJsonModel(n));

                // Отдаем
                return JsonSuccess(items);
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        [HttpPost]
        [AccessAuthorize]
        [ValidateInput(false)]
        public JsonResult Save([ModelBinder(typeof(JsonModelBinder))] CategoryMapJsonModel model, HttpPostedFileBase file)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<ICategoriesMapRepository>();
                var categoriesRepository = Locator.GetService<ICategoriesRepository>();

                CategoryMap categoryMap;

                if (model.Id <= 0)
                {
                    // Создаем новую элемент
                    categoryMap = new CategoryMap
                    {
                        DisplayName = model.DisplayName,
                        DateCreated = DateTime.Now,
                        SortOrder = model.SortOrder
                    };
                    repository.Add(categoryMap);
                }
                else
                {
                    // Обновляем существующую
                    categoryMap = repository.Load(model.Id);
                    if (categoryMap == null)
                    {
                        throw new ObjectNotFoundException(String.Format("Карта категорий с идентификатором {0} не найден", model.Id));
                    }

                    categoryMap.DisplayName = model.DisplayName;
                    categoryMap.SortOrder = model.SortOrder;
                }

                if (file != null && file.ContentLength > 0)
                {
                    categoryMap.Image = repository.UploadImage(file, categoryMap);
                }

                // Сохраняем изменения
                repository.SubmitChanges();

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e);
            }
        }
    }
}
