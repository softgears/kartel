namespace Kartel.Trade.Web.Areas.ControlPanel.Models
{
    /// <summary>
    /// Модель секции в панели управления
    /// </summary>
    public class ControlPanelSectionModel
    {
        /// <summary>
        /// Заголовок секции
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Идентификатор панели секции
        /// </summary>
        public string SectionId { get; set; }

        /// <summary>
        /// Имя файла скрипта
        /// </summary>
        public string ScriptName { get; set; }

        /// <summary>
        /// Создает модель секции панели управления
        /// </summary>
        /// <param name="title">Заголовок секции</param>
        /// <param name="sectionId">Идентификатор родительской панели на которую разместить текущую панель</param>
        /// <param name="scriptName">Имя файла скрипта с логикой, относительно папки Scripts</param>
        /// <returns>Типовая вкладка панели управления</returns>
        public ControlPanelSectionModel(string title, string sectionId, string scriptName)
        {
            Title = title;
            SectionId = sectionId;
            ScriptName = scriptName;
        }
    }
}