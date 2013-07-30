using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Areas.ControlPanel.Classes;
using Kartel.Trade.Web.Areas.ControlPanel.Models;

namespace Kartel.Trade.Web.Areas.ControlPanel.Controllers
{
    /// <summary>
    /// Контроллер управления пользователями
    /// </summary>
    public class ManageUsersController : BaseRootController
    {
        //
        // GET: /ControlPanel/ManageUsers/
        /// <summary>
        /// Отображает страницу управления пользователями
        /// </summary>
        /// <returns></returns>
        [AccessAuthorize]
        public ActionResult Index()
        {
            return ControlPanelSectionView("Управление пользователя","users","Users.js");
        }

        /// <summary>
        /// Возвращает на клиент пользователей
        /// </summary>
        /// <param name="type">Тип пользователей</param>
        /// <param name="start">Начало</param>
        /// <param name="limit">Сколько</param>
        /// <returns></returns>
        [HttpPost][AccessAuthorize()]
        public ActionResult GetUsers(string type, int start, int limit)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IUsersRepository>();

                var allUsers = type == "gold" ? repository.Search(a => a.Tarif == "gold") : repository.FindAll();

                // Список страниц
                var count = allUsers.Count();

                // Отдаем
                return new JsonNetResult()
                    {
                        ContentEncoding = Encoding.UTF8,
                        ContentType = "application/json",
                        Data = new
                            {
                                data = allUsers.Skip(start).Take(limit).Select(n => new UserJsonModel(n)),
                                results = count
                            }
                    };
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        /// <summary>
        /// Сохраняет изменения в выбранном пользователе
        /// </summary>
        /// <param name="model">Модель пользователя</param>
        /// <returns></returns>
        [HttpPost][AccessAuthorize()]
        public ActionResult SaveUser(UserJsonModel model)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IUsersRepository>();

                // Ищем пользователя
                var user = repository.Load(model.Id);
                if (user != null)
                {
                    model.UpdateUser(user);
                }
                repository.SubmitChanges();

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

    }
}
