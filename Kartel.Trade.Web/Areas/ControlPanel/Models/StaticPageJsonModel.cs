using System;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// JSON модель статистической страницы
    /// </summary>
    public class StaticPageJsonModel
    {
        /// <summary>
        /// Идентификатор страницы
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// Заголовок страницы
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Путь к странице относительно корня сайта
        /// </summary>
        [JsonProperty("route")]
        public string Route { get; set; }

        /// <summary>
        /// Аннотация к странице
        /// </summary>
        [JsonProperty("annotation")]
        [AllowHtml()]
        public string Annotation { get; set; }

        /// <summary>
        /// HTML содержимое страницы
        /// </summary>
        [JsonProperty("content")]
        [AllowHtml()]
        public string Content { get; set; }

        /// <summary>
        /// Статус страницы
        /// </summary>
        [JsonProperty("status")]
        public int Status { get; set; }

        /// <summary>
        /// Количество просмотров
        /// </summary>
        [JsonProperty("views")]
        public int Views { get; set; }

        /// <summary>
        /// Дата создания страницы
        /// </summary>
        [JsonProperty("dateCreated")]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Дата редактирования страницы
        /// </summary>
        [JsonProperty("dateModified")]
        public DateTime? DateModified { get; set; }

        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        public StaticPageJsonModel()
        {
        }

        /// <summary>
        /// Конструктор на базе доменного объекта
        /// </summary>
        /// <param name="page">Доменный объект</param>
        public StaticPageJsonModel(StaticPage page)
        {
            Id = page.Id;
            Title = page.Title;
            Route = page.Route;
            Content = page.Content;
            Views = page.Views;
            DateCreated = page.DateCreated;
            DateModified = page.DateModified;
        }
    }
}