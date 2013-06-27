// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	IBillsRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 27.06.2013 16:55
// 
// ============================================================

using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий счетов
    /// </summary>
    public interface IBillsRepository: IBaseRepository<Bill>
    {
         
    }
}