using System;
using System.Collections.Generic;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Lucene.Net.Documents;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория тендеров
    /// </summary>
    public class TendersRepository: BaseRepository<Tender>, ITendersRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public TendersRepository(KartelDataContext dataContext) : base(dataContext)
        {

        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override Tender Load(long id)
        {
            return Find(t => t.Id == id);
        }

        /// <summary>
        /// Возвращает индекс указанных документов
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Document> BuildIndex()
        {
            // Перебираем все тендеры
            foreach (var tender in FindAll())
            {
                // Документ
                var doc = new Document();
                doc.Add(new Field("Id", tender.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("EntityType", "Tender", Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Text", String.Format("{0};{1};{2}", tender.Title, tender.Keywords, tender.Description), Field.Store.YES, Field.Index.ANALYZED));
                yield return doc;
            }
        }
    }
}