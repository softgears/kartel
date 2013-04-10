using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    }
}
