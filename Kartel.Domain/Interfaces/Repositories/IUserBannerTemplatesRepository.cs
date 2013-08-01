// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	IUserBannerTemplatesRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 31.07.2013 15:59
// 
// ============================================================

using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий шаблонов пользовательских баннеров
    /// </summary>
    public interface IUserBannerTemplatesRepository: IBaseRepository<UserBannerTemplate>
    {
         
    }
}