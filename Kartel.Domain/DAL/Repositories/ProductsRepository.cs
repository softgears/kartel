// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	ProductsRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:42
// 
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;
using Lucene.Net.Documents;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория продуктов
    /// </summary>
    public class ProductsRepository: BaseRepository<Product>, IProductsRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public ProductsRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override Product Load(long id)
        {
            return Find(p => p.Id == id);
        }

        /// <summary>
        /// Возвращает указанное случайное количество товаров из системы
        /// </summary>
        /// <param name="count">Количество для возврата</param>
        /// <returns></returns>
        public IEnumerable<Product> GetRandomProducts(int count = 8)
        {
            // Ищем максимальный ИД
            var maxId = DataContext.Products.Max(p => p.Id);
            // Генератор случайных чисел
            var random = new Random(System.Environment.TickCount);

            // Заполняем список
            var resultList = new List<Product>();
            while (resultList.Count < count)
            {
                var product = Load(random.Next(maxId));
                if (product != null)
                {
                    resultList.Add(product);
                }
            }
            // Отдаем
            return resultList;
        }

        /// <summary>
        /// Получает список горячих товаров из указанной категории или из всех категорий. Возвращает определенное количество товаров и уменьшает количество просмотров у пользователя
        /// </summary>
        /// <param name="count">Количество товаров</param>
        /// <param name="category">Категория, если не задана то возращает из всех категорий</param>
        /// <returns></returns>
        public IList<Product> GetHotProductsForView(int count, Category category = null)
        {
            var resultList = new List<Product>();
            var allProducts =
                Search(
                    r =>
                    r.HotProducts != null && r.HotProducts.EnableHotProduct &&
                    r.HotProducts.Product.User.AvailableHotProductsShows > 0);

            // Накладываем фильтр по категориям
            if (category != null)
            {
                var categoriesIds = category.GetAllChildCategories().Select(c => c.Id).ToArray();
                allProducts = allProducts.Where(p => categoriesIds.Contains(p.Id));
            }

            // Сортируем случайно и возращаем указанное количество товаров
            allProducts = allProducts.OrderBy(d => Guid.NewGuid()).Take(count);

            // Добавляем в результирующий список
            resultList.AddRange(allProducts);

            // Перебираем список и уменьшаем количество показов
            foreach (var prod in resultList)
            {
                prod.HotProducts.Views += 1;
                prod.User.AvailableHotProductsShows -= 1;
            }

            // Сохраняем изменения в базу
            SubmitChanges();

            // Отдаем список
            return resultList;
        }

        /// <summary>
        /// Возвращает индекс указанных документов
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Document> BuildIndex()
        {
            // Перебираем все товары
            foreach (var product in FindAll())
            {
                // Документ
                var doc = new Document();
                doc.Add(new Field("Id", product.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("EntityType", "Product", Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Text", String.Format("{0};{1};{2}",product.Title,product.Keywords,product.Description), Field.Store.YES, Field.Index.ANALYZED));
                yield return doc;
            }
        }
    }
}