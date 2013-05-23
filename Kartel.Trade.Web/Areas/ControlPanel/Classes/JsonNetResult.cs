using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kartel.Trade.Web.Areas.ControlPanel.Classes
{
    /// <summary>
    /// Переопределяем стандартный JsonResult для поддержки Json сериализатора Json.NET
    /// </summary>
    public class JsonNetResult: JsonResult
    {
        /// <summary>
        /// Переопределяем исполняющий метод резалта
        /// </summary>
        /// <param name="context">Контекст выполнения запроса</param>
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(this.ToString());
        }

        /// <summary>
        /// Переопределяем метод преобразования класса в строку
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return this.Data != null ? JsonConvert.SerializeObject(this.Data,new JavaScriptDateTimeConverter()) : string.Empty;
        }
    }
}