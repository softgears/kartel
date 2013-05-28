// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	BannerJsonModel.cs
// 
// 	Created by: ykorshev 
// 	 at 27.05.2013 10:33
// 
// ============================================================

using Kartel.Domain.Entities;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// Модель баннера
    /// </summary>
    public class BannerJsonModel
    {
        /// <summary>
        /// Идентификатор баннера
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Картинка
        /// </summary>
        [JsonProperty("img")]
        public string Img { get; set; }

        /// <summary>
        /// HTML код баннера
        /// </summary>
        [JsonProperty("html")]
        public string Html { get; set; }

        /// <summary>
        /// Ссылка при клике на баннер
        /// </summary>
        [JsonProperty("href")]
        public string Href { get; set; }

        /// <summary>
        /// Индекс для сортировки
        /// </summary>
        [JsonProperty("sort")]
        public int Sort { get; set; }

        /// <summary>
        /// ИД товаров, на странице которых отображается баннер
        /// </summary>
        [JsonProperty("objects")]
        public string Objects { get; set; }

        /// <summary>
        /// категории, в которых отображается баннер
        /// </summary>
        [JsonProperty("categories")]
        public string Categories { get; set; }

        /// <summary>
        /// Разделы, в которых отображается баннер
        /// </summary>
        [JsonProperty("extra")]
        public string Extra { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public BannerJsonModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public BannerJsonModel(Banner banner)
        {
            Id = banner.Id;
            Title = banner.Title;
            Img = banner.Img;
            Href = banner.Href;
            Sort = banner.Sort;
            Objects = banner.Objects;
            Extra = banner.Extra;
            Categories = banner.Categories;
        }
    }
}