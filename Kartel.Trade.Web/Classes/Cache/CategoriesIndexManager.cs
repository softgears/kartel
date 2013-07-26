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
        public IDictionary<int, int> CategoriesProductsCountCache { get; private set; }

        /// <summary>
        /// Кеш количества тендеров в категории
        /// </summary>
        public IDictionary<int, int> CategoriesTendersCountCache { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CategoriesIndexManager()
        {
            var categoriesManager = Locator.GetService<ICategoriesRepository>();
            RootCategories = categoriesManager.GetRootCategories().ToList();
            CategoriesProductsCountCache = new ConcurrentDictionary<int, int>();
            CategoriesTendersCountCache = new ConcurrentDictionary<int, int>();

            // Делаем предзагрузку всех категорий
            foreach (var category in categoriesManager.FindAll().Select(c => c.Id))
            {
                CategoriesProductsCountCache[category] = categoriesManager.GetProductsCount(category);
            }
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
            if (CategoriesProductsCountCache.ContainsKey(category.Id))
            {
                return CategoriesProductsCountCache[category.Id];
            }

            // Похоже что нет - кешируем
            var count = Locator.GetService<ICategoriesRepository>().GetProductsCount(category.Id);
            CategoriesProductsCountCache[category.Id] = count;
            return count;
        }

        /// <summary>
        /// Возвращает количество тендеров в указанной категории
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int GetTendersCount(Category category)
        {
            // Проверяем на наличие в кеше
            if (CategoriesTendersCountCache.ContainsKey(category.Id))
            {
                return CategoriesTendersCountCache[category.Id];
            }

            // Похоже что нет - кешируем
            var count = Locator.GetService<ICategoriesRepository>().Load(category.Id).Tenders.Count;
            CategoriesTendersCountCache[category.Id] = count;
            return count;
        }

        /// <summary>
        /// Возвращает имена для дочернего списка категорий
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string[] GetMapNames(int id, int count)
        {
            var cat = Locator.GetService<ICategoriesRepository>().Load(id);
            if (cat.CategoryMapItems.Count > 0)
            {
                return
                    cat.CategoryMapItems.OrderBy(d => d.SortOrder).Select(c => c.CategoryMap.DisplayName).Take(3).ToArray();
            }
            else
            {
                return cat.ChildCategories.Take(count).Select(c => c.Title).ToArray();
            }
        }
    }
}