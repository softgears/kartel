using System;
using System.Text;
using System.Web.Mvc;
using Kartel.Trade.Web.Areas.ControlPanel.Classes;
using Kartel.Trade.Web.Areas.ControlPanel.Models;
using Kartel.Trade.Web.Controllers;

namespace Kartel.Trade.Web.Areas.ControlPanel.Controllers
{
    /// <summary>
    /// Абстрактный контроллер панели управления
    /// </summary>
    public  abstract class BaseRootController : BaseController
    {
        /// <summary>
        /// Возвращает сообщение о том что Json операция выполнена успешно, параллельно возвращая какие либо данные
        /// </summary>
        /// <param name="data">Данные для передачи на клиент, опционально</param>
        /// <returns>Json</returns>
        protected JsonNetResult JsonSuccess(object data = null)
        {
            return new JsonNetResult()
                       {
                           ContentEncoding = Encoding.UTF8,
                           ContentType = "application/json",
                           Data = new
                                      {
                                          success = true,
                                          data = data
                                      }
                       };
        }

        /// <summary>
        /// Возвращает сообщение о том что Json операция провалилась
        /// </summary>
        /// <param name="e">Исключение, которое породило ошибку</param>
        /// <returns></returns>
        protected JsonNetResult JsonErrors(Exception e)
        {
            Response.StatusCode = 500;
            return new JsonNetResult()
                       {
                           ContentEncoding = Encoding.UTF8,
                           ContentType = "application/json",
                           Data = new
                                      {
                                          success = false,
                                          message = e.Message
                                      }
                       };
        }

        /// <summary>
        /// Возвращает сообщение о том что Json операция провалилась
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <returns></returns>
        protected JsonNetResult JsonErrors(string message)
        {
            Response.StatusCode = 500;
            return new JsonNetResult()
                       {
                           ContentEncoding = Encoding.UTF8,
                           ContentType = "application/json",
                           Data = new
                                      {
                                          success = false,
                                          message = message
                                      }
                       };
        }

        /// <summary>
        /// Возвращает вью секции панели управления
        /// </summary>
        /// <param name="title">Заголовок секции</param>
        /// <param name="sectionId">Идентификатор родительской панели на которую разместить текущую панель</param>
        /// <param name="scriptName">Имя файла скрипта с логикой, относительно папки Scripts</param>
        /// <returns>Типовая вкладка панели управления</returns>
        protected ActionResult ControlPanelSectionView(string title, string sectionId, string scriptName)
        {
            return View("ControlPanelSection", new ControlPanelSectionModel(title, sectionId, scriptName));
        }

    }
}
