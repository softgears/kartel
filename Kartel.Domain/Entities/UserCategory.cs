﻿// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	UserCategory.cs
// 
// 	Created by: ykorshev 
// 	 at 20.04.2013 16:14
// 
// ============================================================

using System.Collections.Generic;
using System.Linq;

namespace Kartel.Domain.Entities
{
    /// <summary>
    /// СОзданная пользователем категория
    /// </summary>
    public partial class UserCategory
    {
        /// <summary>
        /// Количество продуктов в пользовательской категории
        /// </summary>
        /// <returns></returns>
        public int GetProductsCount()
        {
            return Products.Count;
        }

        /// <summary>
        /// Продукты, находящиеся в этой категории
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetProducts()
        {
            return Products.OrderBy(p => p.Title);
        }
    }
}