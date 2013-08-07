﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Mailing.Templates;
using Kartel.Domain.Infrastructure.Routing;
using Kartel.Domain.Interfaces.Infrastructure;
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
            PushNavigationChainItem("Главная", "/");

            // Загружаем указанную категорию
            var rep = Locator.GetService<ICategoriesRepository>();
            var cat = rep.Load(id);
            if (cat == null)
            {
                return RedirectToAction("NotFound");
            }

            PushNavigationChainItem(cat.Title, "", true);

            // Отображаем вьху указанной категории
            if (cat.CategoryMaps.Count > 0)
            {
                return View(cat);
            }
            return View("StaticCategories", cat);
        }

        /// <summary>
        /// Просматривает содержимое указанной категории
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="region">Страна для отображения</param>
        /// <param name="page">Страница для отображения</param>
        /// <param name="occupation">Вид деятельности</param>
        /// <returns></returns>
        [Route("browse-category/{id}")]
        public ActionResult BrowseCategory(long id, string region, int page = 0, string occupation = "")
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

            IEnumerable<Product> products = cat.GetProducts();

            // Отображает товары c учётом страны
            if (!String.IsNullOrEmpty(region))
            {
                var countryRep = Locator.GetService<ICountriesRepository>();
                var country = countryRep.GetAllCountries().FirstOrDefault(c => c.Code == region);

                if (country != null)
                {
                    // Создаём временную коллекцию для отфильтрованных товаров
                    products = products.Where(p => p.User.Country.ToLower() == country.Name.ToLower());
                }
            }

            // Фильтр по аккупации
            if (!String.IsNullOrEmpty(occupation) && occupation != "views")
            {
                switch(occupation)
                {
                    case "importer":
                        products =
                            products.Where(
                                p => p.User.UserOccupationInfos != null && p.User.UserOccupationInfos.Importer);
                        break;
                    case "exporter":
                        products =
                            products.Where(
                                p => p.User.UserOccupationInfos != null && p.User.UserOccupationInfos.Exporter);
                        break;
                    case "developer":
                        products =
                            products.Where(
                                p => p.User.UserOccupationInfos != null && p.User.UserOccupationInfos.Developer);
                        break;
                    case "agent":
                        products =
                            products.Where(
                                p => p.User.UserOccupationInfos != null && p.User.UserOccupationInfos.Agent);
                        break;
                    case "distributor":
                        products =
                            products.Where(
                                p => p.User.UserOccupationInfos != null && p.User.UserOccupationInfos.Distributor);
                        break;
                    case "odm":
                        products =
                            products.Where(
                                p => p.User.UserOccupationInfos != null && p.User.UserOccupationInfos.ODM);
                        break;
                    case "oem":
                        products =
                            products.Where(
                                p => p.User.UserOccupationInfos != null && p.User.UserOccupationInfos.OEM);
                        break;
                }
            }

            PushNavigationChainItem(cat.Title, "", true);

            // Отображает вьюху категории с товарами
            ViewBag.page = page;
            ViewBag.totalPages = MathHelper.PagesCount(cat.Products.Count, 9);
            ViewBag.occupation = occupation;
            ViewBag.CurrentRegion = region;
            ViewBag.products = products.Skip(page * 9).Take(9).AsEnumerable();

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
            PushNavigationChainItem("Тендеры", "/tenders", true);

            return View();
        }

        /// <summary>
        /// Отображает список всех тендеров в указанной категории
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <param name="page">Страница</param>
        /// <returns></returns>
        [Route("tenders/category/{id}")]
        public ActionResult TendersCategory(long id, string region = null, string subregion = "", int page = 0)
        {
            // Ищем категорию
            var categoriesRep = Locator.GetService<ICategoriesRepository>();
            var category = categoriesRep.Load(id);
            if (category == null)
            {
                return RedirectToAction("Tenders");
            }

            IEnumerable<Tender> tenders = category.GetTenders(true);

            // Отображает товары c учётом страны
            if (!String.IsNullOrEmpty(region))
            {
                var countryRep = Locator.GetService<ICountriesRepository>();
                var country = countryRep.GetAllCountries().FirstOrDefault(c => c.Code == region);

                if (country != null)
                {
                    // Создаём временную коллекцию для отфильтрованных товаров
                    tenders = tenders.Where(p => p.User.Country.ToLower() == country.Name.ToLower());
                }
            }

            // Фильтр по аккупации
            if (!String.IsNullOrEmpty(subregion) && subregion != "")
            {
                tenders = tenders.Where(p => p.User.Region.ToLower() == subregion);
            }

            // Нав цепочка
            PushNavigationChainItem("Главная", "/");
            PushNavigationChainItem("Тендеры", "/tenders", false);
            PushNavigationChainItem(category.Title, "/", true);

            // Вид
            ViewBag.page = page;
            ViewBag.CurrentRegion = region;
            ViewBag.subregion = subregion;
            ViewBag.tenders = tenders.Skip(20 * page).Take(20);

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
                PushNavigationChainItem("Поиск не доступен", "/", true);
                return View("SearchIndexing");
            }

            // Анализируем то, что мы хотим найти
            PushNavigationChainItem("Результаты поиска", "/", true);
            ViewBag.term = term;
            if (what == "products")
            {
                var searched = manager.SearchProducts(term);
                return View("SearchProducts", searched.ToList());
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

        /// <summary>
        /// Отображает страницу всех товаров
        /// </summary>
        /// <returns></returns>
        [Route("products")]
        public ActionResult AllProducts()
        {
            // Пушим навигационную цепочку
            PushNavigationChainItem("Главная", "/");
            PushNavigationChainItem("Товары", "/products", true);

            return View();
        }

        /// <summary>
        /// Обрабатывает редирект со старой ссылки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("catalog/prod/{id}.html")]
        public ActionResult OldProductHandler(int id)
        {
            return Redirect("/product/" + id);
        }

        /// <summary>
        /// Обрабатывает редирект со старой ссылки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("user/{id}/main.html")]
        public ActionResult OldUserHandler(int id)
        {
            return Redirect("/vendor/" + id);
        }

        /// <summary>
        /// Обрабатывает редирект старой ссылки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("catalog/category/{id}.html")]
        public ActionResult OldCatalogHandler(int id)
        {
            return Redirect("/browse-category/" + id);
        }
    }
}
