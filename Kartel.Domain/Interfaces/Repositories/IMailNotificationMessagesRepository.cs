using System.Collections.Generic;
using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный интерфейс очереди нотифицируемых сообщений
    /// </summary>
    public interface IMailNotificationMessagesRepository: IBaseRepository<MailNotificationMessage>
    {
        /// <summary>
        /// Возвращает список сообщений, находящихся в очереди на отправку
        /// </summary>
        /// <returns></returns>
        IEnumerable<MailNotificationMessage> GetEnqueuedMessages();
    }
}