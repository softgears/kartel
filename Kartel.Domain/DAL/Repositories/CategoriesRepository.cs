// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	CategoriesRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:40
// 
// ============================================================

using System.Collections.Generic;
using System.Linq;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// Репозиторий категорий
    /// </summary>
    public class CategoriesRepository: BaseRepository<Category>, ICategoriesRepository
    {
        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        /// <param name="dataContext"></param>
        public CategoriesRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override Category Load(long id)
        {
            return Find(c => c.Id == id);
        }

        /// <summary>
        /// Возвращает список корневых категорий
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Category> GetRootCategories()
        {
            return Search(c => c.ParentId == 0).OrderBy(c => c.Sort);
        }

        /// <summary>
        /// Возвращает список всех дочерних категорий указанной категории
        /// </summary>
        /// <param name="parentId">Идентификатор родительской</param>
        /// <returns></returns>
        public IEnumerable<Category> GetChildCategories(int parentId)
        {
            return Search(c => c.ParentId == parentId);
        }

        /// <summary>
        /// Возвращает список всех категорий карты категорий
        /// </summary>
        /// <param name="map">Карта</param>
        /// <returns></returns>
        public IEnumerable<Category> GetCateroriesOfMap(CategoryMap map)
        {
            var mapItems = map.CategoryMapItems;
            var categories = FindAll().ToList();
            return mapItems.Select(mapItem => categories.FirstOrDefault(f => f.Id == mapItem.CategoryId)).ToList();
        }

        /// <summary>
        /// Возвращает количество товаров в категории и во всех вложенных категориях
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns></returns>
        public int GetProductsCount(int id)
        {
            var cat = Load(id);
            var allCat = cat.GetAllChildCategories();
            return allCat.Sum(c => c.Products.Count);
        }
    }
}