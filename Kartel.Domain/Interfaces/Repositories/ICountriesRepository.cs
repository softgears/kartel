// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	IProductsRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 07.04.2013 17:37
// 
// ============================================================

using System.Collections.Generic;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Search;

namespace Kartel.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Абстрактный репозиторий продуктов
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Возвращает все страны из XML-файла
        /// </summary>
        IEnumerable<Country> GetAllCountries();
    }
}