using System;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    public class CategoryMapJsonModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("categoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("sortOrder")]
        public int SortOrder { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CategoryMapJsonModel()
        {
        }

        public CategoryMapJsonModel(CategoryMap categoryMap)
        {
            Id = categoryMap.Id;
            CategoryId = categoryMap.CategoryId;
            CategoryName = Locator.GetService<ICategoriesRepository>().Load(categoryMap.CategoryId).Title;
            DisplayName = categoryMap.DisplayName;
            Image = categoryMap.Image;
            SortOrder = categoryMap.SortOrder;
            DateCreated = categoryMap.DateCreated;
        }
    }
}