// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	UniSenderAPI.cs
// 
// 	Created by: ykorshev 
// 	 at 26.06.2013 15:55
// 
// ============================================================

using System.Net;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Misc;
using Kartel.Domain.Interfaces.Infrastructure;

namespace Kartel.Domain.Infrastructure.Mailing.UniSender
{
    /// <summary>
    /// Менеджер API Унисендера
    /// </summary>
    public class UniSenderAPI: IUniSenderAPI
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public UniSenderAPI()
        {
            APIKey = System.Configuration.ConfigurationManager.AppSettings["UniSenderAPIKey"];
        }

        /// <summary>
        /// Ключ API
        /// </summary>
        public string APIKey { get; set; }

        /// <summary>
        /// Регистрирует указанного пользователя в системе UniSender
        /// </summary>
        /// <returns></returns>
        public bool RegisterUser(string email, string login)
        {
            var query =
                string.Format("http://api.unisender.com/ru/api/register?format=json&api_key={0}&email={1}&login={2}&notify=1",
                              APIKey, email, login);

            // Выполняем запрос к сервису
            var client = new WebClient();
            var response = client.DownloadString(query);

            // Преобразуем в JSON
            dynamic obj = new DynamicJsonObject(response);

            return obj.error == null;
        }
    }
}