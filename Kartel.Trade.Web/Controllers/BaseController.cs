using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Classes.Navigation;

namespace Kartel.Trade.Web.Controllers
{
    /// <summary>
    /// Базовый контроллер сайта
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Навигационная цепочка
        /// </summary>
        public NavigationChain NavigationChain { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Mvc.Controller"/> class.
        /// </summary>
        protected BaseController()
        {
            NavigationChain = new NavigationChain();
        }

        /// <summary>
        /// Добавляет новый элемент в навигационную цепочку
        /// </summary>
        /// <param name="title">Заголовок элемента</param>
        /// <param name="url">Ссылка на которую ссылается элемент</param>
        /// <param name="inactive">Элемент неактивен</param>
        protected void PushNavigationChainItem(string title, string url, bool inactive = false)
        {
            NavigationChain.Add(new NavigationChainItem(title,url,inactive));
        }

        #region Сессии пользователей

        /// <summary>
        /// Хранение текущего пользователя
        /// </summary>
        private User _user { get; set; }

        /// <summary>
        /// Текущий авторизованный пользователь
        /// </summary>
        public User CurrentUser
        {
            get
            {
                object fromSess = Session["CurrentUser"];
                if (fromSess == null)
                {
                    return null;
                }
                var userId = (int)fromSess;
                if (_user == null)
                {
                    _user = Locator.GetService<IUsersRepository>().Load(userId);
                }
                return _user;
            }
            set
            {
                Session["CurrentUser"] = value != null ? (object)value.Id : null;
                if (value == null)
                {
                    Session.Remove("CurrentUser");
                }
                _user = value;
            }
        }

        /// <summary>
        /// Является ли текущий пользователь авторизованным
        /// </summary>
        public bool IsAuthentificated
        {
            get { return CurrentUser != null; }
        }

        /// <summary>
        /// Авторизирует текущего пользователя
        /// </summary>
        /// <param name="user">Пользователь которого установить как текущего</param>
        /// <param name="remember">Запомнить ли пользователя</param>
        public void AuthorizeUser(User user, bool remember = true)
        {
            CurrentUser = user;
            if (remember)
            {
                // Устанавливаем собственные авторизационные куки
                var authCookie = new HttpCookie("auth");
                authCookie.Values["identity"] = user.Login;
                authCookie.Values["pass"] = user.PasswordHash;
                authCookie.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(authCookie);
            }
        }

        /// <summary>
        /// Убирает авторизацию текущего пользователя и убирает авторизационные куки если они есть
        /// </summary>
        public void CloseAuthorization()
        {
            CurrentUser = null;

            // убираем куки если они есть
            var authCookie = Response.Cookies["auth"];
            if (authCookie != null)
            {
                authCookie = new HttpCookie("auth")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(authCookie);
            }
        }

        #endregion
    }
}
