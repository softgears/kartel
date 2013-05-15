// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	ISearchManager.cs
// 
// 	Created by: ykorshev 
// 	 at 15.05.2013 14:57
// 
// ============================================================

using System.Collections.Generic;
using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Search
{
    /// <summary>
    /// Абстрактный менеджер поиска
    /// </summary>
    public interface ISearchManager
    {
        /// <summary>
        /// Индекс в процессе перестроения
        /// </summary>
        bool IsIndexingInProgress { get; set; }

        /// <summary>
        /// Осуществляет поиск по товарам. Опционально с учетом указанной категории
        /// </summary>
        /// <param name="term">Фраза для поиска</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <returns>Коллекция найденых товаров</returns>
        IList<Product> SearchProducts(string term, int categoryId = -1);

        /// <summary>
        /// Осуществляет поиск по тендерам. Опционально с учетом указаной категории
        /// </summary>
        /// <param name="term">Фраза для поиска</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <returns>Коллекция найденых тендеров</returns>
        IList<Tender> SearchTenders(string term, int categoryId = -1);

        /// <summary>
        /// Выполняет построения индекса по товарам и тендерам
        /// </summary>
        void BuildIndex();

        /// <summary>
        /// Выполняет очищение индекса по товарам и тендерам
        /// </summary>
        void ClearIndex();

        /// <summary>
        /// Выполняет очистку и построения индекса по товарам и тендерам
        /// </summary>
        void RebuildIndex();

        /// <summary>
        /// Выполняет инициализацию поискового механизма
        /// </summary>
        void Init();
    }
}