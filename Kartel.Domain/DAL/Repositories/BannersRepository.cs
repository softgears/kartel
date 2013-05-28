// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	BannersRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 27.05.2013 10:30
// 
// ============================================================

using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория баннеров
    /// </summary>
    public class BannersRepository: BaseRepository<Banner>, IBannersRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public BannersRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override Banner Load(long id)
        {
            return Find(b => b.Id == id);
        }
    }
}