// ============================================================
// 
// 	Kartel
// 	Kartel.Domain 
// 	CategoriesMapRepository.cs
// 
// 	Created by: ykorshev 
// 	 at 18.06.2013 15:41
// 
// ============================================================

using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Policy;
using System.Web;
using Kartel.Domain.Entities;
using Kartel.Domain.Interfaces.Repositories;

namespace Kartel.Domain.DAL.Repositories
{
    /// <summary>
    /// СУБД реализация репозитория карт категорий
    /// </summary>
    public class CategoriesMapRepository : BaseRepository<CategoryMap>, ICategoriesMapRepository
    {
        /// <summary>
        /// Инициализирует новый инстанс абстрактного репозитория для указанного типа
        /// </summary>
        /// <param name="dataContext"></param>
        public CategoriesMapRepository(KartelDataContext dataContext)
            : base(dataContext)
        {

        }

        /// <summary>
        /// Загружает указанную сущность по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность с указанным идентификатором или Null</returns>
        public override CategoryMap Load(long id)
        {
            return Find(m => m.Id == id);
        }

        /// <summary>
        /// Сохраняет файл изображения в указанную папку
        /// </summary>
        /// <param name="file">Изображение</param>
        /// <param name="map">Карта категорий</param>
        public string UploadImage(HttpPostedFileBase file, CategoryMap map)
        {
            if (file == null || file.ContentLength <= 0)
            {
                throw new ArgumentNullException("file");
            }

            var stream = file.InputStream;
            var image = Image.FromStream(stream);
            stream.Seek(0, SeekOrigin.Begin);

            // Проверка на форматы изображений (JPEG, PNG, GIF)
            if (ImageFormat.Jpeg.Equals(image.RawFormat)
                || ImageFormat.Png.Equals(image.RawFormat)
                || ImageFormat.Gif.Equals(image.RawFormat)
                )
            {
                // Имя файла (GUID + расширение)
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                // Абсолютный путь для сохранения изображения
                var imageDirectory = AppDomain.CurrentDomain.BaseDirectory +
                    ConfigurationManager.AppSettings["FilesUrl"];

                // Полный путь к файлу
                var fullImagePath = imageDirectory + fileName;

                // Пишем файл из потока
                FileStream fileStream = File.Create(fullImagePath, (int)stream.Length);
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, bytesInStream.Length);
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);

                // Относительный путь
                return ConfigurationManager.AppSettings["FilesUrl"] + fileName;
            }

            throw new FormatException("Неверный формат изображения!");
        }
    }
}