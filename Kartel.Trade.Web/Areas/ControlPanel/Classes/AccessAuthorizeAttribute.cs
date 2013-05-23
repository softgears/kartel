using System.Web.Mvc;
using System.Web.Routing;
using Kartel.Trade.Web.Controllers;

namespace Kartel.Trade.Web.Areas.ControlPanel.Classes
{
    /// <summary>
    /// Атрибут накладывающий требования на авторизацию перед доступом к функциональности действий
    /// </summary>
    public class AccessAuthorizeAttribute: ActionFilterAttribute
    {
        /// <summary>
        /// Требуемый уровень доступа
        /// </summary>
        public int RequiredAccessLevel { get; private set; }

        /// <summary>
        /// Иницаилизуер атрибут, который требует обязательной пользовательской авторизации для выполения указанного действия с указанным уровнем доступа
        /// </summary>
        /// <param name="requiredAccessLevel">Требуемый уровень допуска</param>
        public AccessAuthorizeAttribute(int requiredAccessLevel = 900)
        {
            RequiredAccessLevel = requiredAccessLevel;
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Проверяем если у пользователя доступ к указанному функционалу

            // Получаем текущего авторизованного пользователя
            var user = ((BaseController) filterContext.Controller).CurrentUser;
            if (user == null || user.Administrator <= 0)
            {
                // Формируем словарь ля редиректа
                var routeDictionary = new RouteValueDictionary {{"action", "Login"}, {"controller", "Root"}};
                filterContext.Result = new RedirectToRouteResult(routeDictionary);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}