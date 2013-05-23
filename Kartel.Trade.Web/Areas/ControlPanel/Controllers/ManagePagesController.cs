using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Exceptions;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Areas.ControlPanel.Classes;
using Kartel.Trade.Web.Areas.ControlPanel.Models;

namespace Kartel.Trade.Web.Areas.ControlPanel.Controllers
{
    /// <summary>
    /// Контроллер управления статическими страницами
    /// </summary>
    public class ManagePagesController : BaseRootController
    {
        /// <summary>
        /// Отображает корневую страницу управления статистическими картинками
        /// </summary>
        /// <returns>Панель управления</returns>
        public ActionResult Index()
        {
            return ControlPanelSectionView("Управление статичными страницами", "pages", "Pages.js");
        }

        /// <summary>
        /// Загружает на клиент список всех статических страниц
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AccessAuthorize]
        public JsonResult GetPages()
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IStaticPagesRepository>();

                // Список страниц
                var pages = repository.FindAll().OrderByDescending(n => n.DateCreated).Select(n => new StaticPageJsonModel(n));

                // Отдаем
                return JsonSuccess(pages);
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        [HttpPost]
        [AccessAuthorize]
        [ValidateInput(false)]
        public JsonResult Save([ModelBinder(typeof(JsonModelBinder))] StaticPageJsonModel model)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IStaticPagesRepository>();

                if (model.Id <= 0)
                {
                    // Создаем новую страничку
                    var page = new StaticPage()
                    {
                        DateCreated = DateTime.Now,
                        Content = model.Content,
                        Route = model.Route,
                        Title = model.Title,
                        Views = 0
                    };
                    repository.Add(page);

                    // Сохраняем изменения
                    repository.SubmitChanges();

                    // Добавляем запись в таблицу роутов
                    var route = RouteTable.Routes.MapRoute("StaticPage" + page.Id, page.Route,
                                               new { controller = "Main", action = "StaticPage", id = page.Id });
                    RouteTable.Routes.Remove(route);
                    RouteTable.Routes.Insert(0, route);

                }
                else
                {
                    // Обновляем существующую
                    var page = repository.Load(model.Id);
                    if (page == null)
                    {
                        throw new ObjectNotFoundException(String.Format("Страница с идентификатором {0} не найдена", model.Id));
                    }

                    // Если роут был изменен то нам требуется его исправить
                    var routeChanged = model.Route != page.Route;
                    var oldRoute = page.Route;

                    
                    page.Content = model.Content;
                    page.Route = model.Route;
                    page.Title = model.Title;
                    page.DateModified = DateTime.Now;

                    // Сохраняем изменения
                    repository.SubmitChanges();

                    // Изменяем роут если надо
                    if (routeChanged)
                    {
                        Route rowItem = (Route)RouteTable.Routes.Cast<Route>().FirstOrDefault(r => r.Url == oldRoute);
                        if (rowItem != null) rowItem.Url = page.Route;
                    }
                }

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e);
            }
        }

        /// <summary>
        /// Удаляет страницу с указанным идентификатором
        /// </summary>
        /// <param name="id">Идентификатор страницы</param>
        /// <returns></returns>
        [HttpPost]
        [AccessAuthorize]
        public JsonResult Delete(long id)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IStaticPagesRepository>();

                // Загружаем сущность
                var page = repository.Load(id);
                if (page == null)
                {
                    throw new ObjectNotFoundException(String.Format("Страница с идентификатором {0} не найдена", id));
                }

                // Удаляем
                repository.Delete(page);
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
