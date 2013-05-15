using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Search;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий тендеров
    /// </summary>
    public interface ITendersRepository: IBaseRepository<Tender>, IIndexableRepository
    {
         
    }
}