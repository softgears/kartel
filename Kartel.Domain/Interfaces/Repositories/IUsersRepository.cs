// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	IUsersRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:34
// 
// ============================================================

using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий пользователей
    /// </summary>
    public interface IUsersRepository: IBaseRepository<User>
    {
         
    }
}