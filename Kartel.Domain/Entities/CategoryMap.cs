using System;
using System.Linq;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;

namespace Kartel.Domain.Entities
{
    public partial class CategoryMap
    {
        /// <summary>
        /// Сохраняет файл изображения в указанную папку
        /// </summary>
        /// <param name="file">Файл</param>
        public void UploadImage(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
            {
                throw new ArgumentNullException("file");
            }

            // Папка для сохранения файлов, указанная в Web.config
            var configDir = ConfigurationManager.AppSettings["FilesUrl"];

            // Сохраняем изображение
            var fileName = SaveImage(file, configDir);

            // Сохраняем относительный путь к изображению
            Image = configDir + fileName;
            Locator.GetService<ICategoriesMapRepository>().SubmitChanges();
        }

        /// <summary>
        /// Валидирует и сохраняет файл изображения
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="configDir">Папка для сохранения файлов, указанная в Web.config</param>
        /// <returns>Имя файла</returns>
        private string SaveImage(HttpPostedFileBase file, string configDir)
        {
            var stream = file.InputStream;

            // Возможные форматы изображений
            string[] extensions = { ".jpeg", ".jpg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != null && extensions.Contains(fileExtension.ToLower()))
            {
                var image = System.Drawing.Image.FromStream(stream);
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
                    var fullImagePath = AppDomain.CurrentDomain.BaseDirectory + configDir + fileName;

                    // Создаём и пишем файл из потока
                    FileStream fileStream = File.Create(fullImagePath, (int) stream.Length);
                    byte[] bytesInStream = new byte[stream.Length];
                    stream.Read(bytesInStream, 0, bytesInStream.Length);
                    fileStream.Write(bytesInStream, 0, bytesInStream.Length);

                    return fileName;
                }

                throw new FormatException("Неверный формат изображения!");
            }

            throw new FormatException("Неверный формат изображения!");
        }
    }
}