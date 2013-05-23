// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	StaticPagesRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 23.05.2013 12:33
// 
// ============================================================

using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория статических страниц
    /// </summary>
    public class StaticPagesRepository: BaseRepository<StaticPage>, IStaticPagesRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public StaticPagesRepository(KartelDataContext dataContext) : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override StaticPage Load(long id)
        {
            return Find(s => s.Id == id);
        }
    }
}