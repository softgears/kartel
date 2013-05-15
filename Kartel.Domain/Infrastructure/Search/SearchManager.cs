// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	SearchManager.cs
// 
// 	Created by: ykorshev 
// 	 at 15.05.2013 15:02
// 
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.Interfaces.Search;
using Kartel.Domain.IoC;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Directory = System.IO.Directory;
using Version = Lucene.Net.Util.Version;

namespace Kartel.Domain.Infrastructure.Search
{
    /// <summary>
    /// Реализация менеджера поиска
    /// </summary>
    public class SearchManager: ISearchManager
    {
        /// <summary>
        /// Индекс в процессе перестроения
        /// </summary>
        public bool IsIndexingInProgress { get; set; }

        /// <summary>
        /// Таймер, запускающий перестройку поискового индекса
        /// </summary>
        public System.Threading.Timer RebuildTimer { get; set; }

        /// <summary>
        /// Путь к индексным файлам
        /// </summary>
        public string IndexPath { get; set; }

        /// <summary>
        /// Осуществляет поиск по товарам. Опционально с учетом указанной категории
        /// </summary>
        /// <param name="term">Фраза для поиска</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <returns>Коллекция найденых товаров</returns>
        public IList<Product> SearchProducts(string term, int categoryId = -1)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Осуществляет поиск по тендерам. Опционально с учетом указаной категории
        /// </summary>
        /// <param name="term">Фраза для поиска</param>
        /// <param name="categoryId">Идентификатор категории</param>
        /// <returns>Коллекция найденых тендеров</returns>
        public IList<Tender> SearchTenders(string term, int categoryId = -1)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Выполняет построения индекса по товарам и тендерам
        /// </summary>
        public void BuildIndex()
        {
            IsIndexingInProgress = true;
            var added = 0;
            try
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(FSDirectory.Open(IndexPath), analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // Начинаем выборку
                    using (var scope = Locator.BeginNestedHttpRequestScope())
                    {
                        // Продукты
                        var productsRep = Locator.GetService<IProductsRepository>();
                        foreach (var document in productsRep.BuildIndex())
                        {
                            writer.AddDocument(document);
                            added++;
                        }

                        // Тендеры
                        var tendersRep = Locator.GetService<ITendersRepository>();
                        foreach (var document in tendersRep.BuildIndex())
                        {
                            writer.AddDocument(document);
                            added++;
                        }
                    }

                    // Закрываем
                    analyzer.Close();
                    writer.Dispose();
                }

            }
            finally
            {
                IsIndexingInProgress = false;
            }
        }

        /// <summary>
        /// Выполняет очищение индекса по товарам и тендерам
        /// </summary>
        public void ClearIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(FSDirectory.Open(IndexPath), analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    // remove older index entries
                    writer.DeleteAll();

                    // close handles
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                // TODO: добавить логирование
            }
        }

        /// <summary>
        /// Выполняет очистку и построения индекса по товарам и тендерам
        /// </summary>
        public void RebuildIndex()
        {
            if (!IsIndexingInProgress)
            {
                ClearIndex();
                BuildIndex();
            }
        }

        /// <summary>
        /// Выполняет инициализацию поискового механизма
        /// </summary>
        public void Init()
        {
            RebuildTimer = new Timer((state) => RebuildIndex(),this,0,1000*60*60*3);
            IndexPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["IndexDirectory"]);
            IsIndexingInProgress = false;
            // Проверяем что путь существует
            if (!Directory.Exists(IndexPath))
            {
                Directory.CreateDirectory(IndexPath);
            }
        }
    }
}