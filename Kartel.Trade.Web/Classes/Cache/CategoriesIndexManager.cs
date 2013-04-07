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
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public CategoriesIndexManager()
        {
            var categoriesManager = Locator.GetService<ICategoriesRepository>();
            RootCategories = categoriesManager.GetRootCategories().ToList();
        }

        /// <summary>
        /// Ничего не делаем просто чтобы конструктор вызывать
        /// </summary>
        public void Init()
        {
            
        }
    }
}