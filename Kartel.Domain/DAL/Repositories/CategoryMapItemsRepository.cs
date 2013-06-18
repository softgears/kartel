// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	CategoryMapItemsRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 18.06.2013 15:43
// 
// ============================================================

using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория элемент карт категорий
    /// </summary>
    public class CategoryMapItemsRepository: BaseRepository<CategoryMapItem>, ICategoriesMapItemsRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public CategoryMapItemsRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override CategoryMapItem Load(long id)
        {
            return Find(mi => mi.Id == id);
        }
    }
}