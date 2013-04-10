// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	CategoriesIndexManager.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:59
// 
// ============================================================

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;

namespace Kartel.Trade.Web.Classes.Cache
{
    /// <summary>
    /// Менеджер категорий, отображаемых на главной странице
    /// </summary>
    public class CategoriesIndexManager
    {
        /// <summary>
        /// Кеш корневых категорий системы
        /// </summary>
        public IList<Category> RootCategories { get; private set; }

        /// <summary>
        /// Кеш количества товаров в категории
        /// </summary>
        public IDictionary<int, int> CategoriesCountCache { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CategoriesIndexManager()
        {
            var categoriesManager = Locator.GetService<ICategoriesRepository>();
            RootCategories = categoriesManager.GetRootCategories().ToList();
            CategoriesCountCache = new ConcurrentDictionary<int, int>();
        }

        /// <summary>
        /// Ничего не делаем просто чтобы конструктор вызывать
        /// </summary>
        public void Init()
        {
            
        }

        /// <summary>
        /// Возвращает изображение, ассоциированное с категорией
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public string GetCategoryImage(Category category)
        {
            return string.Empty;
        }

        /// <summary>
        /// Возвращает количество товаров в указанной категориии
        /// </summary>
        /// <param name="category">Категория</param>
        /// <returns>Количество товаров в ней</returns>
        public int GetProductsCount(Category category)
        {
            // Проверяем на наличие в кеше
            if (CategoriesCountCache.ContainsKey(category.Id))
            {
                return CategoriesCountCache[category.Id];
            }

            // Похоже что нет - кешируем
            var count = category.Products.Count;
            CategoriesCountCache[category.Id] = count;
            return count;
        }
    }
}