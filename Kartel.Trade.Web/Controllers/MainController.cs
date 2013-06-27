using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Routing;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.Interfaces.Search;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Classes.Utils;

namespace Kartel.Trade.Web.Controllers
{
    /// <summary>
    /// Основной контроллер системы
    /// </summary>
    public class MainController : BaseController
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
        [Route("category/{id}")]
        public ActionResult Category(long id)
        {
            // Пушим навигационную цепочку
            PushNavigationChainItem("Главная","/");

            // Загружаем указанную категорию
            var rep = Locator.GetService<ICategoriesRepository>();
            var cat = rep.Load(id);
            if (cat == null)
            {
                return RedirectToAction("NotFound");
            }

            PushNavigationChainItem(cat.Title,"",true);

            // Отображаем вьху указанной категории
            if (cat.CategoryMaps.Count > 0)
            {
                return View(cat);    
            }
            else
            {
                return View("StaticCategories", cat);
            }
            
        }

        /// <summary>
        /// Просматривает содержимое указанной категории
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="region">Страна для отображения</param>
        /// <param name="page">Страница для отображения</param>
        /// <returns></returns>
        [Route("browse-category/{id}")]
        public ActionResult BrowseCategory(long id, string region, int page = 0)
        {
            // Пушим навигационную цепочку
            PushNavigationChainItem("Главная", "/");

            // Загружаем указанную категорию
            var rep = Locator.GetService<ICategoriesRepository>();
            var cat = rep.Load(id);
            if (cat == null)
            {
                return RedirectToAction("NotFound");
            }

            // Отображает товары c учётом страны
            if (region != null)
            {
                var countryRep = Locator.GetService<ICountriesRepository>();
                var country = countryRep.GetAllCountries().FirstOrDefault(c => c.Code == region);

                if (country != null)
                {
                    // Создаём временную коллекцию для отфильтрованных товаров
                    var tempProducts = new EntitySet<Product>();
                    foreach (var prod in cat.Products)
                    {
                        if (prod.User.Country == country.Name)
                        {
                            tempProducts.Add(prod);
                        }
                    }
                    ViewBag.CurrentRegion = country.Code;
                    cat.Products = tempProducts;
                }
            }

            PushNavigationChainItem(cat.Title, "", true);

            // Отображает вьюху категории с товарами
            ViewBag.page = page;
            ViewBag.totalPages = MathHelper.PagesCount(cat.Products.Count, 9);

            return View(cat);
        }

        /// <summary>
        /// Отображает список всех тендеров, разделенный по категориям
        /// </summary>
        /// <returns></returns>
        [Route("tenders")]
        public ActionResult Tenders()
        {
            // Пушим навигационную цепочку
            PushNavigationChainItem("Главная", "/");
            PushNavigationChainItem("Тендеры", "/tenders",true);

            return View();
        }

        /// <summary>
        /// Отображает список всех тендеров в указанной категории
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="page">Страница</param>
        /// <returns></returns>
        [Route("tenders/category/{id}")]
        public ActionResult TendersCategory(long id,int page = 0)
        {
            // Ищем категорию
            var categoriesRep = Locator.GetService<ICategoriesRepository>();
            var category = categoriesRep.Load(id);
            if (category == null)
            {
                return RedirectToAction("Tenders");
            }

            // Нав цепочка
            PushNavigationChainItem("Главная", "/");
            PushNavigationChainItem("Тендеры", "/tenders", false);
            PushNavigationChainItem(category.Title, "/", true);

            // Вид
            ViewBag.page = page;
            return View(category);
        }

        /// <summary>
        /// Обрабатывает глобальный поиск по сайту
        /// </summary>
        /// <param name="term">Строка для поиска</param>
        /// <param name="what">Что именно ищем - товары или тендеры</param>
        /// <returns>страница с результатами поиска</returns>
        [Route("search")]
        public ActionResult Search(string term, string what)
        {
            // Пушим навигационную цепочку
            PushNavigationChainItem("Главная", "/");

            var manager = Locator.GetService<ISearchManager>();
            if (manager.IsIndexingInProgress)
            {
                // Индекс перестраивается - подождем
                PushNavigationChainItem("Поиск не доступен", "/",true);
                return View("SearchIndexing");
            }

            // Анализируем то, что мы хотим найти
            PushNavigationChainItem("Результаты поиска", "/", true);
            ViewBag.term = term;
            if (what == "products")
            {
                var searched = manager.SearchProducts(term);
                return View("SearchProducts",searched.ToList());
            }
            else if (what == "tenders")
            {
                var searched = manager.SearchTenders(term);
                return View("SearchTenders", searched.ToList());
            }

            // Какие то мазафаки пытаются взломать
            return Content("Fuck the goose");
        }

        /// <summary>
        /// Отображает статическую страницу с указанным идентификатором
        /// </summary>
        /// <param name="id">Идентификатор страницы</param>
        /// <returns></returns>
        public ActionResult StaticPage(long id)
        {
            var rep = Locator.GetService<IStaticPagesRepository>();
            var page = rep.Load(id);
            if (page == null)
            {
                return RedirectToAction("Index");
            }

            return View(page);
        }
    }
}
