// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	BannersRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 27.05.2013 10:30
// 
// ============================================================

using System.Configuration;
using System.IO;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория дополнительных фото товара
    /// </summary>
    public class ProductImagesRepository : BaseRepository<ProductImage>, IProductImagesRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public ProductImagesRepository(KartelDataContext dataContext)
            : base(dataContext)
        {
        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override ProductImage Load(long id)
        {
            return Find(b => b.Id == id);
        }

        public override void Delete(ProductImage image)
        {
            DeleteImageFile(image);
            base.Delete(image);
        }

        private static void DeleteImageFile(ProductImage image)
        {
            var fileName = image.Image;
            var basePath = ConfigurationManager.AppSettings["FilesStoragePath"];
            var subfolder = ConfigurationManager.AppSettings["ProductImagesFolderName"];
            var filePath = basePath + subfolder + fileName;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}