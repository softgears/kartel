using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;

namespace Kartel.Domain.Entities
{
    /// <summary>
    /// Категория с товарами
    /// </summary>
    public partial class Category
    {
        /// <summary>
        /// Возвращает список дочерних категорий
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Category> GetChildCategories()
        {
            return Locator.GetService<ICategoriesRepository>().Search(c => c.ParentId == Id).OrderBy(c => c.Sort); // TODO: переделать на lazy получение через RefEntityTable
        }

        /// <summary>
        /// Возвращает указанное количество случайных товаров из указанной категории
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetRandomProducts(int count = 4)
        {
            return Products.OrderBy(p => Guid.NewGuid()).Take(count);
        }

        /// <summary>
        /// Возвращает продукты в категории
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetProducts()
        {
            return Products;
        }
    }
}