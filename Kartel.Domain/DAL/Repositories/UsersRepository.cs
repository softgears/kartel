// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	UsersRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:44
// 
// ============================================================

using System.Linq;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория пользователей
    /// </summary>
    public class UsersRepository: BaseRepository<User>, IUsersRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public UsersRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override User Load(long id)
        {
            return Find(u => u.Id == id);
        }

        /// <summary>
        /// Проверяет, существует ли в системе пользователь с указанным логином
        /// </summary>
        /// <param name="email">Email для логина</param>
        /// <returns></returns>
        public bool ExistsUserWithLogin(string email)
        {
            return Search(u => u.Login.ToLower() == email.ToLower()).Any();
        }
    }
}