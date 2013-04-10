using System;

namespace Kartel.Trade.Web.Classes.Utils
{
    /// <summary>
    /// Математический помошник
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Вычисляет количество страниц необходимое для отображения указанного количества элементов с указанным количеством элементов на странице
        /// </summary>
        /// <param name="count">Количество элементов</param>
        /// <param name="perPage">Элементов на странице</param>
        /// <returns></returns>
        public static int PagesCount(int count, int perPage)
        {
            if (count % perPage != 0)
            {
                return (int)Math.Floor((decimal)(count / perPage)) + 1;
            }
            else
            {
                return count / perPage;
            }
        }
    }
}