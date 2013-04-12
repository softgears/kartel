using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Misc;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Trade.Web.Models;
using XCaptcha;

namespace Kartel.Trade.Web.Controllers
{
    /// <summary>
    /// Контроллер управления поьзователями и личным кабинетом
    /// </summary>
    public class AccountController : BaseController
    {
        /// <summary>
        /// Репозиторий пользователей
        /// </summary>
        public IUsersRepository UsersRepository { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.Controller"/> class.
        /// </summary>
        public AccountController(IUsersRepository usersRepository)
        {
            UsersRepository = usersRepository;
        }

        #region Регистрация

        /// <summary>
        /// Проверяет уникальность имени пользователя
        /// </summary>
        /// <param name="Email">Email пользователя</param>
        /// <returns></returns>
        public ActionResult CheckLogin(string Email)
        {
            var email = UsersRepository.ExistsUserWithLogin(Email);
            if (email)
            {
                return Content("\"Пользователь с таким Email уже зарегистрирован\"");
            }
            else
            {
                return Content("true");
            }
        }

        /// <summary>
        /// Возвращает каптчу и устанавливает ее ключ в сессию текущего контекста
        /// </summary>
        /// <returns></returns>
        public ActionResult RegCaptcha()
        {
            var builder = new XCaptcha.ImageBuilder()
            {
                Canvas = new DefaultCanvas()
                {
                    Width = 115,
                    Height = 37
                }
            };
            var captcha = builder.Create(new Random(System.Environment.TickCount).Next(65535).ToString());
            Session.Add("reg_captcha", captcha.Solution);
            return new FileContentResult(captcha.Image, captcha.ContentType);
        }

        /// <summary>
        /// Проверяет правильность каптчи регистрации
        /// </summary>
        /// <param name="captcha">Каптча</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckRegCaptcha(string captcha)
        {
            var solution = Session["reg_captcha"];
            if (solution == null || solution.ToString() != captcha)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        /// <summary>
        /// Отображает страницу регистарции на сайте
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            // Навигационная цепочка
            PushNavigationChainItem("Главная страница","/");
            PushNavigationChainItem("Регистрация","",true);

            // Отображаем
            return View();
        }

        /// <summary>
        /// Обрабатывает регистарцию пользователя
        /// </summary>
        /// <param name="model">Модель данных</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(RegistrationModel model)
        {
            // Проверяем уникальность пользователя
            var exists = UsersRepository.ExistsUserWithLogin(model.Email);
            if (exists)
            {
                ViewBag.message = "На сайте уже зарегистрирован пользователь с таким адресом";
                return View("RegistrationResult");
            }

            // Проверяем соответствие кода каптчи
            var solution = Session["reg_captcha"];
            if (solution == null || solution.ToString() != model.CaptchaValue)
            {
                ViewBag.message = "Не правильный код подтверждения каптчи";
                return View("RegistrationResult");
            }

            // Видимо все ок - создаем нового пользователя
            var newUser = new User()
                {
                    Company = model.OrganizationName,
                    Address = model.Address,
                    City = model.City,
                    Region = model.Region,
                    Login = model.Email,
                    Email = model.Email,
                    Phone = String.Format("{0}{1}{2}", model.CountryCode, model.CityCode, model.PhoneNumber),
                    Url = model.Website,
                    PasswordHash = PasswordUtils.QuickMD5(model.Password),
                    Country = model.Country,
                    Date = DateTime.Now,
                    Tarif = "free"
                };
            UsersRepository.Add(newUser);
            UsersRepository.SubmitChanges();
            // TODO: добавить авторизацию пользователя

            // Уведомляем об успешном создании
            ViewBag.message = "Вы были успешно зарегистрированы на нашем сайте. Теперь вы можете войти в личный кабинет и добавить свои товары в наш каталог";
            return View("RegistrationResult");
        }

        #endregion

        #region Авторизация

        /// <summary>
        /// Отображает страницу входа
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            // Навигационная цепочка
            PushNavigationChainItem("Главная страница", "/");
            PushNavigationChainItem("Вход в личный кабинет", "", true);

            if (IsAuthentificated)
            {
                return RedirectToAction("Index"); // Главная страница личного кабинета
            }

            return View();
        }

        /// <summary>
        /// Обрабатывает авторизацию на сайте
        /// </summary>
        /// <param name="model">Модель данных авторизации</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (IsAuthentificated)
            {
                return RedirectToAction("Index");
            }

            // Ищем пользователя
            var user = UsersRepository.GetUserByLoginAndPasswordHash(model.Login, PasswordUtils.QuickMD5(model.Password));
            if (user == null)
            {
                // Выдаем информацию
                this.ModelState.AddModelError("common","Не найдена такая пара имени пользователя и пароля");

                // Навигационная цепочка
                PushNavigationChainItem("Главная страница", "/");
                PushNavigationChainItem("Вход в личный кабинет", "", true);

                return View("Login");
            }

            // Пользователь найден - авторизуемся
            user.LoggedDate = DateTime.Now;
            user.LoggedIP = Request.UserHostAddress;
            UsersRepository.SubmitChanges();

            // Авторизуем пользователя
            AuthorizeUser(user,model.Remember);

            // Перенаправляем в личный кабинет
            return RedirectToAction("Index");

        }

        #endregion

    }
}
