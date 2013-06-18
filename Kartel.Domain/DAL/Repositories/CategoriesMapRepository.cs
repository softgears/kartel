// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	CategoriesMapRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 18.06.2013 15:41
// 
// ============================================================

using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория карт категорий
    /// </summary>
    public class CategoriesMapRepository: BaseRepository<CategoryMap>, ICategoriesMapRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public CategoriesMapRepository(KartelDataContext dataContext) : base(dataContext)
        {
            
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override CategoryMap Load(long id)
        {
            return Find(m => m.Id == id);
        }
    }
}