using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kartel.Trade.Web.Areas.ControlPanel.Classes
{
    /// <summary>
    /// �������������� ����������� JsonResult ��� ��������� Json ������������� Json.NET
    /// </summary>
    public class JsonNetResult: JsonResult
    {
        /// <summary>
        /// �������������� ����������� ����� �������
        /// </summary>
        /// <param name="context">�������� ���������� �������</param>
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(this.ToString());
        }

        /// <summary>
        /// �������������� ����� �������������� ������ � ������
        /// </summary>
        /// <returns>������</returns>
        public override string ToString()
        {
            return this.Data != null ? JsonConvert.SerializeObject(this.Data,new JavaScriptDateTimeConverter()) : string.Empty;
        }
    }
}