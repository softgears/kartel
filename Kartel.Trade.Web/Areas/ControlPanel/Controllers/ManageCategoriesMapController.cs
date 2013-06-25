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
        public JsonResult Save([ModelBinder(typeof(JsonModelBinder))] CategoryMapJsonModel model, HttpPostedFileBase file, int parent, string categoryIds)
        {
            // TODO: refactor
            try
            {
                // Репозиторий
                var repository = Locator.GetService<ICategoriesMapRepository>();
                var categoriesRepository = Locator.GetService<ICategoriesRepository>();
                var mapItemsRepository = Locator.GetService<ICategoriesMapItemsRepository>();
                var mapItems = mapItemsRepository.FindAll().ToList();
                var map = repository.Load(model.Id);

                CategoryMap categoryMap;

                if (model.Id <= 0)
                {
                    // Создаем новый элемент
                    categoryMap = new CategoryMap
                    {
                        DisplayName = model.DisplayName,
                        DateCreated = DateTime.Now,
                        SortOrder = model.SortOrder,
                        CategoryId = parent
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
                    categoryMap.CategoryId = parent;

                    if (categoryIds != null)
                    {
                        var itemsOfMap = mapItems.Where(f => f.CategoryMapId == map.Id);
                        foreach (var item in itemsOfMap)
                        {
                            mapItemsRepository.Delete(item);
                        }

                        if (categoryIds.Length > 0)
                        {
                            var categoryIdsArr = categoryIds.Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                            var categories = categoryIdsArr.Select(id => categoriesRepository.Load(id)).ToList();

                            if (categories.Any())
                            {
                                foreach (var category in categories)
                                {
                                    mapItemsRepository.Add(new CategoryMapItem
                                    {
                                        Category = category,
                                        CategoryMap = map
                                    });
                                }
                            }
                        }
                    }
                }

                if (file != null && file.ContentLength > 0)
                {
                    // Загружаем изображение для карты категорий
                    try
                    {
                        categoryMap.UploadImage(file);
                    }
                    catch (Exception e)
                    {
                        return JsonErrors(e);
                    }
                }

                // Сохраняем изменения
                mapItemsRepository.SubmitChanges();
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
