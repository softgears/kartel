using System.Text;

namespace Kartel.Domain.Entities
{
    /// <summary>
    /// Тендер
    /// </summary>
    public partial class Tender
    {
        /// <summary>
        /// Возвращает количество заявок по тендеру
        /// </summary>
        /// <returns></returns>
        public int GetOffersCount()
        {
            return 0;
        }

        /// <summary>
        /// Возвращает цену указанную на тендер
        /// </summary>
        /// <returns></returns>
        public string GetPrice()
        {
            if (MinPrice == 0 && MaxPrice == 0)
            {
                return "не указана";
            }
            var priceB = new StringBuilder();
            if (MinPrice > 0)
            {
                priceB.AppendFormat("от {0}", MinPrice);
            }
            if (MaxPrice > 0)
            {
                priceB.AppendFormat(" до {0}", MaxPrice);
            }
            if (!string.IsNullOrEmpty(Currency))
            {
                priceB.Append(" " + Currency);
            }
            return priceB.ToString();
        }
    }
}