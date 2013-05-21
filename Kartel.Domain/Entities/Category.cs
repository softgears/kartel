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
            return Products.OrderBy(p => p.Title);
        }

        /// <summary>
        /// возвращает полностью путь категории от самой верхней до нижней
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Category> GetFullCategoriesPath()
        {
            var list = new List<Category>();
            var category = this;
            do
            {
                list.Insert(0,category);
                category = category.ParentCategory;
            } while (category != null);
            return list;
        }

        /// <summary>
        /// Возвращает тендеры
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tender> GetTenders(bool recursive = false)
        {
            if (!recursive)
            {
                return Tenders;    
            }
            else
            {
                return Tenders.Union(from category in ChildCategories
                                     from tender in category.GetTenders(true)
                                     select tender);
            }
            
        }
    }
}