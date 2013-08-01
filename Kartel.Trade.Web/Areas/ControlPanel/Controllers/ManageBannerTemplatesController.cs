using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Entities;
using Kartel.Domain.Infrastructure.Exceptions;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Areas.ControlPanel.Classes;
using Kartel.Trade.Web.Areas.ControlPanel.Models;
using Kartel.Trade.Web.Classes.Utils;

namespace Kartel.Trade.Web.Areas.ControlPanel.Controllers
{
    /// <summary>
    /// Контроллер управления шаблонами баннеров
    /// </summary>
    public class ManageBannerTemplatesController : BaseRootController
    {
        //
        // GET: /ControlPanel/ManageBannerTemplates/
        [AccessAuthorize]
        public ActionResult Index()
        {
            return ControlPanelSectionView("Шаблоны баннеров", "bannerTemplates", "BannerTemplates.js");
        }

        /// <summary>
        /// Загружает на клиент список всех статических страниц
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AccessAuthorize]
        public JsonResult GetBanners()
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IUserBannerTemplatesRepository>();

                // Список страниц
                var banners = repository.FindAll().Select(n => new BannerTemplateJsonModel(n));

                // Отдаем
                return JsonSuccess(banners);
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        [HttpPost]
        [AccessAuthorize]
        [ValidateInput(false)]
        public JsonResult Save([ModelBinder(typeof(JsonModelBinder))] BannerTemplateJsonModel model)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IUserBannerTemplatesRepository>();

                // Сохраняем файл с картинкой
                bool fileSubmitted = false;
                string submittedFileName = "";
                var file = Request.Files["file"];
                if (file != null && file.ContentLength > 0 && file.ContentType.Contains("image"))
                {
                    fileSubmitted = true;
                    var fileName = String.Format("{0}-{1}{2}", Path.GetFileNameWithoutExtension(file.FileName),
                                                 new Random(System.Environment.TickCount).Next(65535),
                                                 Path.GetExtension(file.FileName));
                    FileUtils.SavePostedFile(file, "bannertemplates", fileName);
                    submittedFileName = fileName;
                }

                if (model.Id <= 0)
                {
                    // Создаем новую страничку
                    var banner = new UserBannerTemplate()
                    {
                        Category = model.Category,
                        Filename = fileSubmitted ? submittedFileName : model.Filename
                    };
                    repository.Add(banner);

                    // Сохраняем изменения
                    repository.SubmitChanges();

                }
                else
                {
                    // Обновляем существующую
                    var banner = repository.Load(model.Id);
                    if (banner == null)
                    {
                        throw new ObjectNotFoundException(String.Format("Баннер с идентификатором {0} не найден", model.Id));
                    }

                    banner.Category = model.Category;
                    banner.Filename = fileSubmitted ? submittedFileName : model.Filename;

                    // Сохраняем изменения
                    repository.SubmitChanges();
                }

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e);
            }
        }

        /// <summary>
        /// Удаляет страницу с указанным идентификатором
        /// </summary>
        /// <param name="id">Идентификатор страницы</param>
        /// <returns></returns>
        [HttpPost]
        [AccessAuthorize]
        public JsonResult Delete(long id)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<IUserBannerTemplatesRepository>();

                // Загружаем сущность
                var banner = repository.Load(id);
                if (banner == null)
                {
                    throw new ObjectNotFoundException(String.Format("Баннер с идентификатором {0} не найден", id));
                }

                // Удаляем
                repository.Delete(banner);
                repository.SubmitChanges();

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e);
            }
        }
    }
}
