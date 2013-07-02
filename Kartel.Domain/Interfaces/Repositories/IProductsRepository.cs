// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	IProductsRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:37
// 
// ============================================================

using System.Collections.Generic;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Search;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий продуктов
    /// </summary>
    public interface IProductsRepository: IBaseRepository<Product>, IIndexableRepository
    {
        /// <summary>
        /// Возвращает указанное случайное количество товаров из системы
        /// </summary>
        /// <param name="count">Количество для возврата</param>
        /// <returns></returns>
        IEnumerable<Product> GetRandomProducts(int count = 8);

        /// <summary>
        /// Получает список горячих товаров из указанной категории или из всех категорий. Возвращает определенное количество товаров и уменьшает количество просмотров у пользователя
        /// </summary>
        /// <param name="count">Количество товаров</param>
        /// <param name="category">Категория, если не задана то возращает из всех категорий</param>
        /// <returns></returns>
        IList<Product> GetHotProductsForView(int count, Category category = null);
    }
}