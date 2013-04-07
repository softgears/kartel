// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	ProductsRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:42
// 
// ============================================================

using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория продуктов
    /// </summary>
    public class ProductsRepository: BaseRepository<Product>, IProductsRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public ProductsRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override Product Load(long id)
        {
            return Find(p => p.Id == id);
        }
    }
}