// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	UserBannerTemplatesRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 31.07.2013 16:00
// 
// ============================================================

using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория шаблонов баннеров
    /// </summary>
    public class UserBannerTemplatesRepository: BaseRepository<UserBannerTemplate>, IUserBannerTemplatesRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public UserBannerTemplatesRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override UserBannerTemplate Load(long id)
        {
            return Find(r => r.Id == id);
        }
    }
}