using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Kartel.Domain.Infrastructure.Misc;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Areas.ControlPanel.Classes;

namespace Kartel.Trade.Web.Areas.ControlPanel.Controllers
{
    /// <summary>
    /// Корневой контроллер управления
    /// </summary>
    public class RootController : BaseRootController
    {
        //
        // GET: /ControlPanel/Root/
        /// <summary>
        /// Возвращает корневой контроллер панели управления
        /// </summary>
        /// <returns></returns>
        [AccessAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Возвращает структуру навигационного меню панели управления в виде JSON
        /// </summary>
        /// <returns>Json</returns>
        [HttpPost]
        [AccessAuthorize]
        public JsonResult GetNavMenu()
        {
            var menuItems = new List<Dictionary<string, object>>()
                                {
                                    // Контроллер управления статистических страниц
                                    new Dictionary<string, object>()
                                        {
                                            {"id", "pages"},
                                            {"leaf", true},
                                            {"text", "Статические страницы"},
                                            {"url", "/ControlPanel/ManagePages/"},
                                            {"objectType", "section"}
                                        },
                                    // Контроллер управления продуктами
                                    new Dictionary<string, object>()
                                        {
                                            {"id", "categories"},
                                            {"leaf", true},
                                            {"text", "Категории"},
                                            {"url", "/ControlPanel/ManageCategories/"},
                                            {"objectType", "section"}
                                        },
                                    // Контроллер управления продуктами
                                    new Dictionary<string, object>()
                                        {
                                            {"id", "categoriesMap"},
                                            {"leaf", true},
                                            {"text", "Карта категорий"},
                                            {"url", "/ControlPanel/ManageCategoriesMap/"},
                                            {"objectType", "section"}
                                        },
                                    // Контроллер управления баннерами на главной
                                    new Dictionary<string, object>()
                                        {
                                            {"id", "banners"},
                                            {"leaf", true},
                                            {"text", "Баннеры"},
                                            {"url", "/ControlPanel/ManageBanners/"},
                                            {"objectType", "section"}
                                        },
                                    // Контроллер управления выставленными счетами
                                    new Dictionary<string, object>()
                                        {
                                            {"id", "bills"},
                                            {"leaf", true},
                                            {"text", "Выставленные счета"},
                                            {"url", "/ControlPanel/ManageBills/"},
                                            {"objectType", "section"}
                                        },
                                    // Контроллер управления настройками
                                    new Dictionary<string, object>()
                                        {
                                            {"id", "settings"},
                                            {"leaf", true},
                                            {"text", "Настройки"},
                                            {"url", "/ControlPanel/ManageSettings/"},
                                            {"objectType", "section"}
                                        },
                                };
            return Json(menuItems);
        }

        /// <summary>
        /// Отображает страницу с требованием авторизоваться
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Пытается авторизоваться по логину и паролю
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Если все ок то перекинет на главную страницу админки</returns>
        [HttpPost]
        public ActionResult Login(string login, string password)
        {
            // Репозиторий
            var usersRepository = Locator.GetService<IUsersRepository>();

            // Пытаемся найти пользователя по логину и паролю
            var user = usersRepository.GetUserByLoginAndPasswordHash(login, PasswordUtils.QuickMD5(password));
            if (user == null)
            {
                return View("AccessDenied");
            }

            // Авторизуем пользователя
            AuthorizeUser(user);
            usersRepository.SubmitChanges();

            // Похоже все ок - отправляем пользователя на главную
            return RedirectToAction("Index");
        }
    }
}
