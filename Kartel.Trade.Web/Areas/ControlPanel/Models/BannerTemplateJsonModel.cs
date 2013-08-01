// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	BannerTemplateJsonModel.cs
// 
// 	Created by: ykorshev 
// 	 at 31.07.2013 16:04
// 
// ============================================================

using Kartel.Domain.Entities;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// JSON модель баннера
    /// </summary>
    public class BannerTemplateJsonModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public BannerTemplateJsonModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public BannerTemplateJsonModel(UserBannerTemplate banner)
        {
            Id = banner.Id;
            Filename = banner.Filename;
            Category = banner.Category;
        }

        /// <summary>
        /// Обнолвяет объект баннера
        /// </summary>
        /// <param name="banner"></param>
        public void UpdateBanner(UserBannerTemplate banner)
        {
            banner.Filename = Filename;
            banner.Category = Category;
        }
    }
}