// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	User.cs
// 
// 	Created by: ykorshev 
// 	 at 20.04.2013 11:10
// 
// ============================================================

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kartel.Domain.Entities
{
    /// <summary>
    /// Зарегистрированный пользователь системы
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// Легальная информация о пользователе
        /// </summary>
        /// <returns></returns>
        public UserLegalInfo GetLegalInfo()
        {
            if (UserLegalInfos == null)
            {
                UserLegalInfos = new UserLegalInfo()
                    {
                        User = this
                    };
            }
            return UserLegalInfos;
        }

        /// <summary>
        /// Возвращает категории пользователя
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserCategory> GetUserCategories()
        {
            return UserCategories;
        }

        /// <summary>
        /// Возвращает список тендеров
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tender> GetTenders()
        {
            return Tenders;
        }

        /// <summary>
        /// Отображает список всех отправленых заявок на участие в тендерах
        /// </summary>
        /// <returns></returns>
        public IList<TenderOffer> GetTenderOffers()
        {
            return TenderOffers.OrderBy(to => to.DateCreated).ToList();
        }
    }
}