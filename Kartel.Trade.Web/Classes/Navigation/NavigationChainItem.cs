namespace Kartel.Trade.Web.Classes.Navigation
{
    /// <summary>
    /// Элемент навигационной цепочки
    /// </summary>
    public class NavigationChainItem
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Элемент неактивен
        /// </summary>
        public bool Inactive { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public NavigationChainItem(string title, string url, bool inactive = false)
        {
            Title = title;
            Url = url;
            Inactive = inactive;
        }
    }
}