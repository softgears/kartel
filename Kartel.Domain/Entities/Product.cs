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

using System.Collections.Generic;
using System.Linq;
using Kartel.Domain.DAL;
using Kartel.Domain.DAL.Repositories;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;

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

        /// <summary>
        /// Возвращаем цену на товар
        /// </summary>
        /// <returns></returns>
        public string GetPrice()
        {
            if (!string.IsNullOrEmpty(Price))
            {
                return Price;
            }
            else if (!string.IsNullOrEmpty(Field3))
            {
                var parts = Field3.Split('|');
                return parts[0];
            }
            return string.Empty;
        }

        /// <summary>
        /// Возвращает валюту товара
        /// </summary>
        /// <returns></returns>
        public string GetCurrency()
        {
            if (!string.IsNullOrEmpty(Currency))
            {
                return Currency;
            }
            else if (!string.IsNullOrEmpty(Field3))
            {
                var parts = Field3.Split('|');
                if (parts.Length >= 2)
                {
                    return parts[1];
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Возвращает дополнительные фотографии товара
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductImage> GetProductImages()
        {
            var repository = Locator.GetService<IProductImagesRepository>();
            var images = repository.FindAll().Where(f => f.ProductId == Id);
            return images;
        }
    }
}