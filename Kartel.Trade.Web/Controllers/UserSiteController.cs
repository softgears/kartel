using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Routing;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;

namespace Kartel.Trade.Web.Controllers
{
    /// <summary>
    /// Контроллер отображения пользовательского сайта
    /// </summary>
    public class UserSiteController : BaseController
    {
        /// <summary>
        /// Репозиторий пользователей
        /// </summary>
        public IUsersRepository UsersRepository { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.Controller"/> class.
        /// </summary>
        public UserSiteController(IUsersRepository usersRepository)
        {
            UsersRepository = usersRepository;
        }

        /// <summary>
        /// Отображает главную страницу сайта пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [Route("vendor/{id}")]
        public ActionResult Index(long id)
        {
            // Инициализируем пользователя
            InitializeUser(id);
            return View();
        }

        /// <summary>
        /// Инициаилизирует пользователя во вьюбаг
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        private void InitializeUser(long id)
        {
            // Загружаем пользователя
            var user = UsersRepository.Load(id);
            if (user == null)
            {
                // Редиректим
                Response.Redirect("/",true);
                return;
            }
            ViewBag.user = user;

            // Проверяем каталог его товаров
            if (user.UserCategories.Count == 0 && user.Products.Count > 0)
            {
                var startPos = 0;
                // Добавляем пользователю категории товаров
                foreach(var product in user.Products)
                {
                    // Ищем категорию
                    var category = user.UserCategories.FirstOrDefault(uc => uc.Title == product.Category.Title);
                    if (category == null)
                    {
                        // СОздаем
                        startPos += 1000;
                        category = new UserCategory()
                            {
                                Title = product.Category.Title,
                                Description = String.Empty,
                                Position = (startPos),
                                User = user
                            };
                        user.UserCategories.Add(category);
                    }
                    // Добавляем в нее товар
                    category.Products.Add(product);
                }
            }
            // Сохраняем
            UsersRepository.SubmitChanges();
        }
    }
}
