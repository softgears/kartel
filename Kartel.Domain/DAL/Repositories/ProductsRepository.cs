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
    }
}