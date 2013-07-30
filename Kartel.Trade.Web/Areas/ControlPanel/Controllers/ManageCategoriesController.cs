using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Exceptions;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Areas.ControlPanel.Classes;
using Kartel.Trade.Web.Areas.ControlPanel.Models;

namespace Kartel.Trade.Web.Areas.ControlPanel.Controllers
{
    /// <summary>
    /// Контроллер управления категориями системы
    /// </summary>
    public class ManageCategoriesController : BaseRootController
    {
        [AccessAuthorize()]
        public ActionResult Index()
        {
            return ControlPanelSectionView("Управление категориями", "categories", "Categories.js");
        }

        /// <summary>
        /// Загружает древо категорий на клиент
        /// </summary>
        /// <returns>Все категории системы в JSON структуре</returns>
        public JsonResult GetCategories()
        {
            try
            {
                // Перечисляем категории
                var categories = new List<ExtJSTreeNodeModel>();
                EnumerateCategories(0, categories);

                // Отдаем результат
                return new JsonNetResult()
                {
                    Data = categories,
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception e)
            {
                return JsonErrors(e);
            }
        }

        /// <summary>
        /// Получает подкатегории первого уровня
        /// </summary>
        /// <returns></returns>
        [AccessAuthorize]
        public JsonResult GetFirstLevelSubCategories(int? mapId)
        {
            var repository = Locator.GetService<ICategoriesRepository>();
            var  selectedMapItems = new List<CategoryMapItem>();

            if (mapId != null)
            {
                var mapRepository = Locator.GetService<ICategoriesMapRepository>();
                var map = mapRepository.Load((int) mapId);
                var mapItems = map.CategoryMapItems;
                selectedMapItems = mapItems.Where(f => f.CategoryMapId == mapId).ToList();
            }

            var parentCategories = repository.FindAll().Where(f => f.ParentId == 0).ToList();
            var childCategories = new List<object>();

            foreach (var parentCategory in parentCategories)
            {
                foreach (var childCategory in parentCategory.ChildCategories)
                {
                    childCategories.Add(new
                    {
                        id = childCategory.Id,
                        title = childCategory.Title,
                        parentId = childCategory.ParentId,
                        parentTitle = repository.Load(childCategory.ParentId).Title,
                        selected = selectedMapItems.Any(f => f.CategoryId == childCategory.Id)
                    });
                }
            }

            return JsonSuccess(childCategories);
        }

        /// <summary>
        /// Получает родительские категории
        /// </summary>
        /// <returns></returns>
        [AccessAuthorize]
        public JsonResult GetParentCategories()
        {
            var repository = Locator.GetService<ICategoriesRepository>();
            var parentCategories = repository.FindAll().Where(f => f.ParentId == 0).ToList();
            var result = new List<object>();
            foreach (var parentCategory in parentCategories)
            {
                result.Add(new
                {
                    id = parentCategory.Id,
                    title = parentCategory.Title
                });
            }

            return JsonSuccess(result);
        }

        /// <summary>
        /// Рекурсивная функция для построения иерархии объектов в списке
        /// </summary>
        /// <param name="parentId">Идентификатор родительской категории</param>
        /// <param name="categoriesList">Список куда помещать выбранные категории</param>
        private void EnumerateCategories(int parentId, List<ExtJSTreeNodeModel> categoriesList)
        {
            var repository = Locator.GetService<ICategoriesRepository>();
            categoriesList.AddRange(repository.GetChildCategories(parentId).Select(c => new CategoryTreeNodeModel()
                {
                    Expanded = false,
                    Id = c.Id,
                    Text = c.Title,
                    Tooltip = "",
                    Leaf = false,
                    Category =
                        {
                            Description = "",
                            DisplayName = c.Title,
                            SystemName = "",
                            Id = c.Id.ToString(),
                            ParentId = c.ParentId.ToString()
                        },
                    Childrens = new List<ExtJSTreeNodeModel>()
                }));
            foreach (CategoryTreeNodeModel category in categoriesList)
            {
                EnumerateCategories(category.Id, category.Childrens);
            }
        }

        /// <summary>
        /// Сохраняет изменения в узле категории или создает новую категорию с указанными параметрами
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="parentId">Идентификатор родительской категории</param>
        /// <param name="displayName">Отображаемое имя категории</param>
        /// <param name="systemName">Системное имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="image">Ссылка на изображение, связанное с категорией</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Save(int id, int parentId, string displayName, string systemName, string description, string image)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<ICategoriesRepository>();

                // Обрабатываем запрос
                if (id < 0)
                {
                    // проверяем, существует ли родительская категория, к которой хотят поместить новую категорию
                    if (parentId != -1 && repository.Load(parentId) == null)
                    {
                        throw new ObjectNotFoundException("Категории, указанная в качестве родительской не существует");
                    }

                    // Создаем категорию
                    var category = new Category()
                    {
                        ParentId = parentId,
                        Title = displayName
                    };
                    repository.Add(category);
                    repository.SubmitChanges();
                    return JsonSuccess(new { id = category.Id });
                }
                else
                {
                    var category = repository.Load(id);
                    category.Title = displayName;
                    repository.SubmitChanges();
                    return JsonSuccess(new { id = category.Id });
                }
            }
            catch (Exception e)
            {
                return JsonErrors(e);
            }
        }

        /// <summary>
        /// Удаляет категорию и все в нее вложенные категории
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns></returns>
        [HttpPost]
        [AccessAuthorize()]
        public JsonResult Delete(long id)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<ICategoriesRepository>();

                // Проверяем что нам есть что удалять
                var category = repository.Load(id);
                if (category == null)
                {
                    throw new ObjectNotFoundException(string.Format("Категория с идентификатором {0} не найдена", id));
                }

                // Удаляем
                repository.Delete(category);
                repository.SubmitChanges();

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e);
            }
        }

        /// <summary>
        /// Обрабатывает запрос на перемещение категории от одного родителя к другому
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="newParentId">Идентификатор новой родительской категории</param>
        /// <returns>Информация об успехе или нет</returns>
        [HttpPost]
        [AccessAuthorize()]
        public JsonResult Move(int id, int newParentId)
        {
            try
            {
                // Получаем репозиторий
                var repository = Locator.GetService<ICategoriesRepository>();

                // проверяем, существует ли родительская категория, к которой хотят поместить новую категорию
                if (newParentId != -1 && repository.Load(newParentId) == null)
                {
                    throw new ObjectNotFoundException("Категории, указанная в качестве родительской не существует");
                }

                // Загружаем категорию
                var category = repository.Load(id);
                if (category == null)
                {
                    throw new ObjectNotFoundException("Перемещаемая категория не найдена");
                }

                // Изменяем родителя и сохраняем
                category.ParentId = newParentId;
                repository.SubmitChanges();

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e);
            }
        }

        /// <summary>
        /// Возвращает на клиент списко всех категорий
        /// </summary>
        /// <returns></returns>
        [AccessAuthorize()]
        public ActionResult GetAllCategories()
        {
            try
            {
                // Получаем репозиторий
                var repository = Locator.GetService<ICategoriesRepository>();

                var cats = repository.FindAll().Select(c => new
                    {
                        id = c.Id,
                        title = c.Title
                    });

                return JsonSuccess(cats);
            }
            catch (Exception e)
            {
                return JsonErrors(e);
            }
        }
    }
}
