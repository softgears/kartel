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

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий продуктов
    /// </summary>
    public interface IProductsRepository: IBaseRepository<Product>
    {
        /// <summary>
        /// Возвращает указанное случайное количество товаров из системы
        /// </summary>
        /// <param name="count">Количество для возврата</param>
        /// <returns></returns>
        IEnumerable<Product> GetRandomProducts(int count = 8);
    }
}