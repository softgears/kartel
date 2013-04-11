using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Interfaces.Repositories;
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

        #endregion

    }
}
