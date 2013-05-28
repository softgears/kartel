// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	IBannersRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 27.05.2013 10:29
// 
// ============================================================

using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий баннеров
    /// </summary>
    public interface IBannersRepository: IBaseRepository<Banner>
    {
         
    }
}