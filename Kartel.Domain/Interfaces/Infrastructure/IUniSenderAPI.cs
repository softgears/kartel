// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	IUniSenderAPI.cs
// 
// 	Created by: ykorshev 
// 	 at 26.06.2013 15:54
// 
// ============================================================

using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Infrastructure
{
    /// <summary>
    /// Менеджер API для интеграции с UniSender
    /// </summary>
    public interface IUniSenderAPI
    {
        /// <summary>
        /// Регистрирует указанного пользователя в системе UniSender
        /// </summary>
        /// <returns></returns>
        bool RegisterUser(string email, string login);
    }
}