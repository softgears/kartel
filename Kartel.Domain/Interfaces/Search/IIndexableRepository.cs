using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Kartel.Domain.Interfaces.Search
{
    /// <summary>
    /// Абстрактный репозиторий, который позволяет индексировать свое содержимое
    /// </summary>
    public interface IIndexableRepository
    {
        /// <summary>
        /// Возвращает индекс указанных документов
        /// </summary>
        /// <returns></returns>
        IEnumerable<Document> BuildIndex();
    }
}