// ============================================================
// 
// 	Kartel
// 	Kartel.Trade.Web 
// 	FileUtils.cs
// 
// 	Created by: ykorshev 
// 	 at 28.04.2013 15:22
// 
// ============================================================

using System.Configuration;
using System.IO;
using System.Web;
using Kartel.Domain.Entities;

namespace Kartel.Trade.Web.Classes.Utils
{
    /// <summary>
    /// Вспомогательные методы обработки файлов
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Выполняет сохранение
        /// </summary>
        /// <param name="file">Файл, который нужно сохранить</param>
        /// <param name="subfolder">Дополнительная подпапка</param>
        /// <param name="fileName">Имя файла, под которым сохранить</param>
        public static void SavePostedFile(HttpPostedFileBase file, string subfolder, string fileName)
        {
            var basePath = ConfigurationManager.AppSettings["FilesStoragePath"];
            var filePath = Path.Combine(basePath, subfolder, fileName);
            file.SaveAs(filePath);
        }
    }
}