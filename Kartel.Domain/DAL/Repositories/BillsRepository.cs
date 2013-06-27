// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	BillsRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 27.06.2013 16:56
// 
// ============================================================

using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория счетов
    /// </summary>
    public class BillsRepository: BaseRepository<Bill>, IBillsRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public BillsRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override Bill Load(long id)
        {
            return Find(b => b.Id == id);
        }
    }
}