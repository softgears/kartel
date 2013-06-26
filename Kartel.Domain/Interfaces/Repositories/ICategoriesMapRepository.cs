// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	ICategoriesMapRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 18.06.2013 15:39
// 
// ============================================================

using System.Web;
using Kartel.Domain.Entities;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий карт категорий
    /// </summary>
    public interface ICategoriesMapRepository: IBaseRepository<CategoryMap>
    {
    }
}