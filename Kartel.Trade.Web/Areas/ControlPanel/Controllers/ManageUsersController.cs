using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Mailing.Templates;
using Kartel.Domain.Interfaces.Infrastructure;
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

        /// <summary>
        /// Возвращает на клиент все продукты пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost][AccessAuthorize]
        public ActionResult GetProducts(int userId)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IUsersRepository>();

                var user = repository.Load(userId);

                return JsonSuccess(user.Products.Select(p => new ProductJsonModel(p)));
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        /// <summary>
        /// Удаляет указанный товар
        /// </summary>
        /// <param name="id">Товар</param>
        /// <returns></returns>
        [HttpPost][AccessAuthorize]
        public ActionResult DeleteProduct(int id)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IProductsRepository>();

                var prod = repository.Load(id);
                repository.Delete(prod);
                repository.SubmitChanges();

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        /// <summary>
        /// Создает или сохраняет товар
        /// </summary>
        /// <param name="model">Товар</param>
        /// <returns></returns>
        [HttpPost]
        [AccessAuthorize]
        public ActionResult SaveProduct(ProductJsonModel model)
        {
            var rep = Locator.GetService<IProductsRepository>();
            try
            {
                if (model.Id <= 0)
                {
                    var newProduct = new Product()
                    {
                        Date = DateTime.Now
                    };
                    model.UpdateProduct(newProduct);
                    rep.Add(newProduct);
                    rep.SubmitChanges();
                }
                else
                {
                    var prod = rep.Load(model.Id);
                    model.UpdateProduct(prod);
                    prod.Date = DateTime.Now;
                    rep.SubmitChanges();
                }

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        /// <summary>
        /// Возвращает на клиент все тендеры пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost][AccessAuthorize]
        public ActionResult GetTenders(int userId)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IUsersRepository>();

                var user = repository.Load(userId);

                return JsonSuccess(user.Tenders.Select(p => new TenderJsonModel(p)));
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        /// <summary>
        /// Удаляет указанный тендер
        /// </summary>
        /// <param name="id">Товар</param>
        /// <returns></returns>
        [HttpPost][AccessAuthorize]
        public ActionResult DeleteTender(int id)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<ITendersRepository>();

                var prod = repository.Load(id);
                repository.Delete(prod);
                repository.SubmitChanges();

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        /// <summary>
        /// Создает или сохраняет тендер
        /// </summary>
        /// <param name="model">Товар</param>
        /// <returns></returns>
        [HttpPost]
        [AccessAuthorize]
        public ActionResult SaveTender(TenderJsonModel model)
        {
            var rep = Locator.GetService<ITendersRepository>();
            try
            {
                if (model.Id <= 0)
                {
                    var newTender = new Tender()
                    {
                        DateCreated = DateTime.Now
                    };
                    model.UpdateTender(newTender);
                    rep.Add(newTender);
                    rep.SubmitChanges();
                }
                else
                {
                    var tender = rep.Load(model.Id);
                    model.UpdateTender(tender);
                    rep.SubmitChanges();
                }

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        /// <summary>
        /// Отправляет сообщение указанной компании
        /// </summary>
        /// <param name="id">Идентификатор компании</param>
        /// <param name="type">Тип сообщения</param>
        /// <returns></returns>
        [AccessAuthorize()][HttpPost]
        public ActionResult SendMessage(long id, string type)
        {
            try
            {
                var rep = Locator.GetService<IUsersRepository>();
                var user = rep.Load(id);

                string template = "";
                switch (type)
                {
                    case "gold":
                        template = "GoldenVendor.html";
                        break;
                    case "renew":
                        template = "RenewalRater.html";
                        break;
                }

                // Формируем путь
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Mail", template);

                var temp = new ParametrizedFileTemplate(path, user).ToString();

                Locator.GetService<IMailNotificationManager>().Notify(user,"Сообщение с сайта Картель.рф",temp);

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }
        
    }
}
