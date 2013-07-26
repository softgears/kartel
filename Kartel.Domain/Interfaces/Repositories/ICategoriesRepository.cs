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

        /// <summary>
        /// Возвращает список всех дочерних категорий указанной категории
        /// </summary>
        /// <param name="parentId">Идентификатор родительской</param>
        /// <returns></returns>
        IEnumerable<Category> GetChildCategories(int parentId);

        /// <summary>
        /// Возвращает список всех категорий карты категорий
        /// </summary>
        /// <param name="map">Карта</param>
        /// <returns></returns>
        IEnumerable<Category> GetCateroriesOfMap(CategoryMap map);

        /// <summary>
        /// Возвращает количество товаров в категории и во всех вложенных категориях
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns></returns>
        int GetProductsCount(int id);
    }
}