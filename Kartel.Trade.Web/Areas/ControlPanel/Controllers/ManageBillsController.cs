using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Infrastructure.Exceptions;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Areas.ControlPanel.Classes;
using Kartel.Trade.Web.Areas.ControlPanel.Models;

namespace Kartel.Trade.Web.Areas.ControlPanel.Controllers
{
    /// <summary>
    /// Контроллер управления выставленными счетами
    /// </summary>
    public class ManageBillsController : BaseRootController
    {
        //
        // GET: /ControlPanel/ManageBills/
        /// <summary>
        /// Отображает панель управления выставленными счетами
        /// </summary>
        /// <returns></returns>
        [AccessAuthorize()]
        public ActionResult Index()
        {
            return ControlPanelSectionView("Панель управления счета","bills","Bills.js");
        }

        /// <summary>
        /// Возвращает на клиент сведения о всех счетах в системе
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AccessAuthorize()]
        public ActionResult GetBills()
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IBillsRepository>();

                // Список страниц
                var bills = repository.FindAll().OrderByDescending(n => n.DateCreated).Select(n => new BillJsonModel(n));

                // Отдаем
                return JsonSuccess(bills);
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        /// <summary>
        /// Помечает указанный счет как оплаченный и активирует по нему услугу
        /// </summary>
        /// <param name="id">Идентификатор счета</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActivateBill(long id)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IBillsRepository>();

                // Ищем счет
                var bill = repository.Load(id);
                if (bill == null)
                {
                    throw new ObjectNotFoundException(String.Format("Счет с идентификатором {0} не найден",id));
                }

                // Проверяем что счет уже был оплачен или активирован
                if (bill.Activated)
                {
                    throw new ObjectNotFoundException(String.Format("Счет с идентификатором {0} уже был активирован", id));
                }

                // Помечаем счет как оплаченный
                bill.Payed = true;
                repository.SubmitChanges();

                switch (bill.ActivationTarget)
                {
                    case "tariff":
                        // Активируем золотого поставщика
                        bill.User.Tarif = "gold";
                        bill.User.TariffExpiration = DateTime.Now.AddMonths(bill.ActivationAmount);

                        // Помечаем счет как активированный
                        bill.Activated = true;

                        Locator.GetService<IUsersRepository>().SubmitChanges();
                        break;
                    case "hot-products":
                        // Добавляем показов для указанного горячего товара
                        bill.User.AvailableHotProductsShows += bill.ActivationAmount;

                        // Помечаем счет как активированный
                        bill.Activated = true;

                        Locator.GetService<IProductsRepository>().SubmitChanges();

                        break;
                        
                    case "banners":
                        // TODO: сделать активацию баннера
                        break;
                }

                // Отдаем
                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }
    }
}
