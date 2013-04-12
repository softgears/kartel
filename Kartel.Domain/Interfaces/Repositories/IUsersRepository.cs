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
        /// <summary>
        /// Проверяет, существует ли в системе пользователь с указанным логином
        /// </summary>
        /// <param name="email">Email для логина</param>
        /// <returns></returns>
        bool ExistsUserWithLogin(string email);

        /// <summary>
        /// Ищет в базе данных пользователя по комбинации логина и пароля
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="passwordHash">Хеш пароля</param>
        /// <returns></returns>
        User GetUserByLoginAndPasswordHash(string login, string passwordHash);
    }
}