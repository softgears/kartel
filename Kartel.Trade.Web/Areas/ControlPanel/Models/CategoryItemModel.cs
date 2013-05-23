using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// Модель узла категорий
    /// </summary>
    public class CategoryItemModel
    {
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Системное имя
        /// </summary>
        [JsonProperty("systemName")]
        public string SystemName { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Скрыт ли узел
        /// </summary>
        [JsonProperty("image")]
        public string Image { get; set; }

        /// <summary>
        /// Идентификатор узла
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Родительский элемент
        /// </summary>
        [JsonProperty("parentId")]
        public string ParentId { get; set; }
    }
}