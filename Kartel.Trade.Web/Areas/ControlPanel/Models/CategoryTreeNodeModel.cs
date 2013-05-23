using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// Модель элемента дерева категорий
    /// </summary>
    public class CategoryTreeNodeModel: ExtJSTreeNodeModel
    {
        /// <summary>
        /// Целочисленный идентификатор
        /// </summary>
        [JsonProperty("id")]
        public new int Id { get; set; }
        
        /// <summary>
        /// Описание категории
        /// </summary>
        [JsonProperty("category")]
        public CategoryItemModel Category { get; private set; }

        /// <summary>
        /// Создает новый для дерева категории
        /// </summary>
        public CategoryTreeNodeModel()
        {
            Category = new CategoryItemModel();
        }
    }
}