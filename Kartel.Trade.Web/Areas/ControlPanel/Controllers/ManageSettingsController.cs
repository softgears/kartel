using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kartel.Domain.Interfaces.Repositories;
using Kartel.Domain.IoC;
using Kartel.Trade.Web.Areas.ControlPanel.Classes;
using Kartel.Trade.Web.Areas.ControlPanel.Models;

namespace Kartel.Trade.Web.Areas.ControlPanel.Controllers
{
    /// <summary>
    /// Контроллер управления настройками системы
    /// </summary>
    public class ManageSettingsController : BaseRootController
    {
        /// <summary>
        /// Отображает корневую страницу панели управления настройками системы
        /// </summary>
        /// <returns></returns>
        [AccessAuthorize]
        public ActionResult Index()
        {
            return ControlPanelSectionView("Настройки сайта","settings","Settings.js");
        }

        /// <summary>
        /// Загружает на клиент список всех статических страниц
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AccessAuthorize]
        public JsonResult GetSettings()
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<ISettingsRepository>();

                // Список страниц
                var settings = repository.FindAll();
                var components = new List<object>();

                // Перебираем все настройки и формируем клиентский конфиг
                foreach(var setting in settings.Where(t => t.Type != null))
                {
                    object config = null;
                    switch (setting.Type)
                    {

                        case "text":
                            config = new
                                {
                                    xtype = "textfield",
                                    fieldLabel = setting.DisplayName,
                                    name = setting.Key,
                                    allowBlank = true,
                                    id = String.Format("setting{0}field",setting.Key),
                                    anchor = "100%",
                                    value = setting.Value
                                };
                            break;
                        case "textarea":
                            config = new
                                {
                                    xtype = "textarea",
                                    fieldLabel = setting.DisplayName,
                                    name = setting.Key,
                                    allowBlank = true,
                                    id = String.Format("setting{0}field",setting.Key),
                                    anchor = "100%",
                                    height = 70,
                                    value = setting.Value
                                };
                            break;
                    }
                    if (config != null)
                    {
                        components.Add(config);
                    }
                }

                // Отдаем
                return JsonSuccess(new
                    {
                        components = components
                    });
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }

        /// <summary>
        /// Сохраняет изменения в настройках
        /// </summary>
        /// <param name="collection">Коллекция форм</param>
        /// <returns></returns>
        [HttpPost][AccessAuthorize()]
        public ActionResult Save(FormCollection collection)
        {
            try
            {
                // Репозиторий
                var repository = Locator.GetService<ISettingsRepository>();

                // Перебираем все что прислали и сохраняем
                foreach (var key in collection.AllKeys)
                {
                    if (repository.HasSetting(key))
                    {
                        var val = collection[key] ?? String.Empty;
                        repository.SetValue(key,val);
                    }
                }
                repository.SubmitChanges();

                return JsonSuccess();
            }
            catch (Exception e)
            {
                return JsonErrors(e.Message);
            }
        }
    }
}
