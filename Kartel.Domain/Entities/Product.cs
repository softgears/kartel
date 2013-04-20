// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	Product.cs
// 
// 	Created by: ykorshev 
// 	 at 20.04.2013 18:44
// 
// ============================================================
namespace Kartel.Domain.Entities
{
    /// <summary>
    /// Товар
    /// </summary>
    public partial class Product
    {
        /// <summary>
        /// Возвращает оставшееся оплаченное количество оплаченных показов в секции горячих товаров для этого товара
        /// </summary>
        /// <returns></returns>
        public int GetPayedViewsCount()
        {
            return HotProducts != null ? HotProducts.PayedViews : 0;
        }

        /// <summary>
        /// Возвращает количество кликов по данному товару
        /// </summary>
        /// <returns></returns>
        public int GetClicksCount()
        {
            return HotProducts != null ? HotProducts.Clicks : 0;
        }

        /// <summary>
        /// Возвращает общее количество показов, которое имел данный товаров в секции горячих товаров
        /// </summary>
        /// <returns></returns>
        public int GetTotalViewsCount()
        {
            return HotProducts != null ? HotProducts.Views : 0;   
        }
    }
}