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
    }
}