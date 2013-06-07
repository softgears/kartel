﻿using System.Collections.Generic;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория очереди сообщений
    /// </summary>
    public class MailNotificationMessagesRepository: BaseRepository<MailNotificationMessage>, IMailNotificationMessagesRepository
    {
        /// <summary>
        /// Стандартный конструктор
        /// </summary>
        /// <param name="dataContext">Контекст доступа к данным</param>
        public MailNotificationMessagesRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override MailNotificationMessage Load(long id)
        {
            return Find(m => m.Id == id);
        }

        /// <summary>
        /// Возвращает список сообщений, находящихся в очереди на отправку
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MailNotificationMessage> GetEnqueuedMessages()
        {
            return Search(m => m.Sended == false);
        }
    }
}