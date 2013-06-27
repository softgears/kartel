// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	BillJsonModel.cs
// 
// 	Created by: ykorshev 
// 	 at 27.06.2013 17:02
// 
// ============================================================

using System;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// JSON модель счета
    /// </summary>
    public class BillJsonModel
    {
        /// <summary>
        /// ИД счета
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [JsonProperty("user")]
        public string User { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        [JsonProperty("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Счет оплачен
        /// </summary>
        [JsonProperty("payed")]
        public bool Payed { get; set; }

        /// <summary>
        /// Счет оплачен
        /// </summary>
        [JsonProperty("payedTitle")]
        public string PayedTitle
        {
            get { return Payed ? "Оплачены" : "Не оплачены"; }
        }

        /// <summary>
        /// Вид услуги по счету
        /// </summary>
        [JsonProperty("activationTarget")]
        public string ActivationTarget { get; set; }

        /// <summary>
        /// Размер услуги, длительность
        /// </summary>
        [JsonProperty("activationAmount")]
        public string ActivationAmount { get; set; }

        /// <summary>
        /// Цель активируемой услуги
        /// </summary>
        [JsonProperty("activationTargetId")]
        public string ActivationTargetId { get; set; }

        /// <summary>
        /// Услуга активирована
        /// </summary>
        [JsonProperty("activated")]
        public bool Activated { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [JsonProperty("dateCreated")]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Дата редактирования
        /// </summary>
        [JsonProperty("dateModified")]
        public DateTime? DateModified { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public BillJsonModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public BillJsonModel(Bill bill)
        {
            Id = bill.Id;
            User = String.Format("{0} ({1})", bill.User.Company, bill.User.FIO);
            Amount = bill.Amount.ToString("0");
            Payed = bill.Payed;
            switch (bill.ActivationTarget)
            {
                case "tariff":
                    ActivationTarget = "Тариф 'Золотой Поставщик'";
                    ActivationAmount = String.Format("на {0} дней", bill.ActivationAmount);
                    break;
                case "hot-products":
                    ActivationTarget = "Оплата показа Горячих товаров";
                    ActivationAmount = String.Format("{0} показов", bill.ActivationAmount);
                    var prod = Locator.GetService<IProductsRepository>().Load(bill.ActivationTargetId);
                    ActivationTargetId = String.Format("товар {0}", prod != null ? prod.Title : "");
                    break;
                case "banners":
                    ActivationTarget = "Создание баннера";
                    break;
            }
            Activated = bill.Activated;
            DateCreated = bill.DateCreated;
            DateModified = bill.DateActivated;
        }
    }
}