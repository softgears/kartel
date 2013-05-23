using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// Базовая модель узла дерева ExtJS 
    /// </summary>
    public class ExtJSTreeNodeModel
    {
        /// <summary>
        /// Идентификатор узла
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Текст в узле
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Вспылывающая подсказка к узлу
        /// </summary>
        [JsonProperty("qtip")]
        public string Tooltip { get; set; }

        /// <summary>
        /// Класс иконки
        /// </summary>
        [JsonProperty("iconCls")]
        public string IconClass { get; set; }

        /// <summary>
        /// Ссылка на файл иконки
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// URL для перехода при клике на узле
        /// </summary>
        [JsonProperty("href")]
        public string Href { get; set; }

        /// <summary>
        /// Является ли узел не имеющим потомков
        /// </summary>
        [JsonProperty("leaf")]
        public bool Leaf { get; set; }

        /// <summary>
        /// Раскрыт ли узел по умолчанию
        /// </summary>
        [JsonProperty("expanded")]
        public bool Expanded { get; set; }

        /// <summary>
        /// Дочерние узлы
        /// </summary>
        [JsonProperty("children")]
        public List<ExtJSTreeNodeModel> Childrens { get; set; }

        /// <summary>
        /// Рендерить ли чекбокс + состояние чекбокса
        /// </summary>
        [JsonProperty("checked")]
        public bool? Checked { get; set; }
    }
}