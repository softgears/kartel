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

using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий продуктов
    /// </summary>
    public interface IProductsRepository: IBaseRepository<Product>
    {
         
    }
}