// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	IStaticPagesRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 23.05.2013 12:32
// 
// ============================================================

using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий статических страниц
    /// </summary>
    public interface IStaticPagesRepository: IBaseRepository<StaticPage>
    {
         
    }
}