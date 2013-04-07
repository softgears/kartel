// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	ICategoriesRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:35
// 
// ============================================================

using System.Collections.Generic;
using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий категорий
    /// </summary>
    public interface ICategoriesRepository: IBaseRepository<Category>
    {
        /// <summary>
        /// Возвращает список корневых категорий
        /// </summary>
        /// <returns></returns>
        IEnumerable<Category> GetRootCategories();
    }
}