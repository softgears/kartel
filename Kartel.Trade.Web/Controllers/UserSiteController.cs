using System;
using System.Collections.Generic;
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
        /// Обрабатывает
        /// </summary>
        /// <param name="subdomain">Обрабатывает редирект на пользовательский сайт при заходе через поддомен</param>
        /// <returns></returns>
        public ActionResult Subdomain(string subdomain)
        {
            // Ищем пользователя по поддомену
            var user = UsersRepository.Find(u => u.Subdomain != null && u.Subdomain.ToLower() == subdomain.ToLower());
            if (user == null)
            {
                // Пользователь не найден
                return RedirectToAction("Index", "Main");
            }
            
            return RedirectToAction("Index", new {id = user.Id});
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

            // Навигационная цепочка
            PushNavigationChainItem("Главная", string.Format("/vendor/{0}", id));

            return View();
        }

        /// <summary>
        /// Инициаилизирует пользователя во вьюбаг
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        private User InitializeUser(long id)
        {
            // Загружаем пользователя
            var user = UsersRepository.Load(id);
            if (user == null)
            {
                // Редиректим
                Response.Redirect("/",true);
                return null;
            }
            ViewBag.user = user;

            // Проверяем каталог его товаров
            if (user.UserCategories.Count == 0 && user.Products.Count > 0)
            {
                var startPos = 0;
                // Добавляем пользователю категории товаров
                foreach(var product in user.Products.Where(p => p.Category != null))
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

            return user;
        }

        /// <summary>
        /// Отображает страницу со всеми товарами пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [Route("vendor/products/{id}")]
        public ActionResult Products(long id)
        {
            // Инициализируем пользователя
            InitializeUser(id);

            // Навигационная цепочка
            PushNavigationChainItem("Главная",string.Format("/vendor/{0}", id));
            PushNavigationChainItem("Товары",string.Format("/vendor/products/{0}", id),true);
            
            return View();
        }

        /// <summary>
        /// Отображает страницу со всеми товарами пользователя в указаной пользовательской категории
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="catId">Идентификатор категории пользователя</param>
        /// <param name="page">Текущая страница</param>
        /// <returns></returns>
        [Route("vendor/category/{id}")]
        public ActionResult CategoryProducts(long id,long catId, int page = 0)
        {
            // Инициализируем пользователя
            var user = InitializeUser(id);

            // Проверяем категорию
            var userCategory = user.UserCategories.FirstOrDefault(uc => uc.Id == catId);
            if (userCategory == null)
            {
                return RedirectToAction("Index", new {id = id});
            }

            // Навигационная цепочка
            PushNavigationChainItem("Главная", string.Format("/vendor/{0}", id));
            PushNavigationChainItem("Товары", string.Format("/vendor/products/{0}", id));
            PushNavigationChainItem(userCategory.Title, string.Format("/vendor/category/{0}?catId={1}", id,catId),true);

            // Отображаем вид
            ViewBag.page = page;
            return View(userCategory);
        }

        /// <summary>
        /// Отображает страницу указанного товара
        /// </summary>
        /// <param name="id">Идентификатор товара</param>
        /// <returns></returns>
        [Route("product/{id}")]
        public ActionResult ViewProduct(long id)
        {
            // Ищем товар
            var product = Locator.GetService<IProductsRepository>().Load(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Main");
            }

            // Подгатавливаем пользователя
            InitializeUser(product.UserId);

            // Навигационная цепочка
            PushNavigationChainItem("Главная", string.Format("/vendor/{0}", id));
            PushNavigationChainItem("Товары", string.Format("/vendor/products/{0}", id));
            if (product.UserCategory != null)
                PushNavigationChainItem(product.UserCategory.Title, string.Format("/vendor/category/{0}?catId={1}", product.UserId, product.UserCategoryId));
            PushNavigationChainItem(product.Title, string.Format("/product/{0}", product.Id),true);

            // Отображаем вид
            return View(product);
        }

        /// <summary>
        /// Отображает страницу с информацией о компании
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("vendor/about/{id}")]
        public ActionResult About(long id)
        {
            // Инициализируем пользователя
            InitializeUser(id);

            // Навигационная цепочка
            PushNavigationChainItem("Главная", string.Format("/vendor/{0}", id));
            PushNavigationChainItem("О компании", string.Format("/vendor/about/{0}", id), true);

            return View();
        }

        /// <summary>
        /// Отображает страницу контактов указанной фирмы
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("vendor/contacts/{id}")]
        public ActionResult Contacts(long id)
        {
            // Инициализируем пользователя
            InitializeUser(id);

            // Навигационная цепочка
            PushNavigationChainItem("Главная", string.Format("/vendor/{0}", id));
            PushNavigationChainItem("О компании", string.Format("/vendor/about/{0}", id), true);

            return View();
        }

        /// <summary>
        /// Обрабатывает пользовательский поиск
        /// </summary>
        /// <param name="term">Идентификатор поискового запроса</param>
        /// <param name="id">Идентификатор пользователя у которого осуществляется поиск</param>
        /// <returns></returns>
        [Route("vendor/search/{id}")]
        public ActionResult Search(string term, long id)
        {
            // Инициализируем пользотваеля
            InitializeUser(id);

            // Навигационная цепочка
            PushNavigationChainItem("Главная", string.Format("/vendor/{0}", id));
            PushNavigationChainItem("Результаты поиска", string.Format("/vendor/search/{0}", id), true);

            // Осуществляем поиск
            var manager = Locator.GetService<ISearchManager>();
            var products = manager.SearchProducts(term).Where(p => p.UserId == id);

            // Отдаем вид
            ViewBag.term = term;
            return View(products);
        }

        /// <summary>
        /// Обрабатывает подачу отправку сообщения указанному пользователю
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <param name="Name">Ваше имя</param>
        /// <param name="Email">Ваш email</param>
        /// <param name="Content">Текст вопроса</param>
        /// <returns></returns>
        [HttpPost][Route("vendor/feedback/{id}")]
        public ActionResult Feedback(long id, string Name, string Email, string Content, string Subject)
        {
            // Инициализируем пользотваеля
            InitializeUser(id);

            // Навигационная цепочка
            PushNavigationChainItem("Главная", string.Format("/vendor/{0}", id));
            PushNavigationChainItem("Отправка сообщения", string.Format("/vendor/feedback", id), true);
            
            // Формируем шаблон
            var template =
                new ParametrizedFileTemplate(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Mail", "Feedback.html"), new
                        {
                            Subject = Subject,
                            Email = Email,
                            Content = Content,
                            Name = Name,
                            IP = Request.UserHostAddress
                        });

            var user = UsersRepository.Load(id);
            if (user == null)
            {
                return RedirectToAction("Index",new {id = id});
            }

            Locator.GetService<IMailNotificationManager>().Notify(user,"Картель.рф: "+Subject,template.ToString());

            return View();
        }

        /// <summary>
        /// Обрабатывает форму обратной связи для задания вопроса по товару
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("product-feedback/{id}")]
        public ActionResult ProductFeedback(long id)
        {
            // Ищем товар
            var product = Locator.GetService<IProductsRepository>().Load(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Main");
            }

            // Подгатавливаем пользователя
            InitializeUser(product.UserId);

            // Навигационная цепочка
            PushNavigationChainItem("Главная", string.Format("/vendor/{0}", id));
            PushNavigationChainItem("Товары", string.Format("/vendor/products/{0}", id));
            if (product.UserCategory != null)
                PushNavigationChainItem(product.UserCategory.Title, string.Format("/vendor/category/{0}?catId={1}", product.UserId, product.UserCategoryId));
            PushNavigationChainItem(product.Title, string.Format("/product/{0}", product.Id), false);
            PushNavigationChainItem("Задать вопрос", string.Format("/product-feedback/{0}", product.Id), true);

            // Отображаем вид
            return View(product);
        }
    }
}
