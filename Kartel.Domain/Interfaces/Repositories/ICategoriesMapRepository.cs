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
        /// <summary>
        /// Сохраняет файл изображения в указанную папку
        /// </summary>
        /// <param name="file">Изображение</param>
        /// <param name="map">Карта категорий</param>
        string UploadImage(HttpPostedFileBase file, CategoryMap map);
    }
}